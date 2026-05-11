//Autor: Fábio Henrique Cabrini
//Resumo: Esse programa possibilita ligar e desligar o led onboard, além de mandar o status para o Broker MQTT possibilitando o Helix saber
//se o led está ligado ou desligado.
//Revisões:
//Rev1: 26-08-2023 Código portado para o ESP32 e para realizar a leitura de luminosidade e publicar o valor em um tópico aprorpiado do broker 
//Autor Rev1: Lucas Demetrius Augusto 
//Rev2: 28-08-2023 Ajustes para o funcionamento no FIWARE Descomplicado
//Autor Rev2: Fábio Henrique Cabrini
//Rev3: 1-11-2023 Refinamento do código e ajustes para o funcionamento no FIWARE Descomplicado
//Autor Rev3: Fábio Henrique Cabrini
#include <WiFi.h>
#include <PubSubClient.h>
#include "DFRobotDFPlayerMini.h"

HardwareSerial myHardwareSerial(2);
DFRobotDFPlayerMini myDFPlayer;

// Configurações - variáveis editáveis
const int default_audioVolume = 30; // Volume do módulo DFPlayer (range: 0-30)
const char* default_SSID = "Wokwi-GUEST"; // Nome da rede Wi-Fi
const char* default_PASSWORD = ""; // Senha da rede Wi-Fi
const char* default_BROKER_MQTT = ""; // IP do Broker MQTT
const int default_BROKER_PORT = 1883; // Porta do Broker MQTT
const char* default_TOPICO_SUBSCRIBE = "/TEF/lamp001/cmd"; // Tópico MQTT de escuta
const char* default_TOPICO_PUBLISH_1 = "/TEF/lamp001/attrs"; // Tópico MQTT de envio de informações para Broker
const char* default_TOPICO_PUBLISH_2 = "/TEF/lamp001/attrs/l"; // Tópico MQTT de envio de informações para Broker
const char* default_ID_MQTT = "fiware_001"; // ID MQTT
const char* topicPrefix = "lamp001"; // Declaração da variável para o prefixo do tópico
// Conexões no ESP32
const int potPin = 34; // Pino do potenciometro
const int busyPin = 4; // Pino conectado ao BUSY do DFPlayer
const int rxPin = 16; // Pino RX2 conectado ao RX do DFPlayer
const int txPin = 17; // Pino TX2 conectado ao RX do DFPlayer
const int D4 = 2; // Pino do LED onboard (conexão embutida na placa)
// Pinos do LED RGB
const int redPin = 21;
const int greenPin = 19;
const int bluePin = 18;

// Variáveis para configurações editáveis
int audioVolume = default_audioVolume;
char* SSID = const_cast<char*>(default_SSID);
char* PASSWORD = const_cast<char*>(default_PASSWORD);
char* BROKER_MQTT = const_cast<char*>(default_BROKER_MQTT);
int BROKER_PORT = default_BROKER_PORT;
char* TOPICO_SUBSCRIBE = const_cast<char*>(default_TOPICO_SUBSCRIBE);
char* TOPICO_PUBLISH_1 = const_cast<char*>(default_TOPICO_PUBLISH_1);
char* TOPICO_PUBLISH_2 = const_cast<char*>(default_TOPICO_PUBLISH_2);
char* ID_MQTT = const_cast<char*>(default_ID_MQTT);

// Configurações PWM
const int frequency = 5000;
const int resolution = 8; // 8 bits: 0-255

WiFiClient espClient;
PubSubClient MQTT(espClient);
char EstadoSaida = '0';

// Função para definir cor e intensidade
void setRGB(int r, int g, int b, float intensity=1) {
  // A intensidade reduz o brilho de cada canal PWM
  ledcWrite(redPin, r * intensity);
  ledcWrite(greenPin, g * intensity);
  ledcWrite(bluePin, b * intensity);
}

void initSerial() {
  Serial.begin(115200);
}

void initWiFi() {
  delay(10);
  Serial.println("------Conexao WI-FI------");
  Serial.print("Conectando-se na rede: ");
  Serial.println(SSID);
  Serial.println("Aguarde");
  reconectWiFi();
}

void initMQTT() {
  MQTT.setServer(BROKER_MQTT, BROKER_PORT);
  MQTT.setCallback(mqtt_callback);
}

void initMP3() {
  pinMode(busyPin, INPUT);

  Serial.println("Compilado!");

  // Inicializa a serial do DFPlayer (9600 baud rate é o padrão dele)
  myHardwareSerial.begin(9600, SERIAL_8N1, rxPin, txPin);

  Serial.println("Iniciando comunicação com o DFPlayer...");

  // Tenta inicializar o módulo
  if (!myDFPlayer.begin(myHardwareSerial)) {
    Serial.println("Erro: Verifique as conexões ou o Cartão SD!");
    while(true); // Trava aqui se houver erro
  }

  Serial.println("DFPlayer conectado");
  delay(3000); // <--- ESPERE O CARTÃO "MONTAR"

  myDFPlayer.volume(audioVolume);
  myDFPlayer.EQ(0);
  delay(500); // Pequena pausa entre comandos
}

void setup() {
  InitOutput();
  initSerial();
  initMP3();
  initWiFi();
  initMQTT();
  MQTT.publish(TOPICO_PUBLISH_1, "s|on");
}

void loop() {
    VerificaConexoesWiFIEMQTT();
    EnviaEstadoOutputMQTT();
    handleLuminosity();
    MQTT.loop();
}

void reconectWiFi() {
    if (WiFi.status() == WL_CONNECTED)
        return;
    WiFi.begin(SSID, PASSWORD);
    while (WiFi.status() != WL_CONNECTED) {
        delay(100);
        Serial.print(".");
    }
    Serial.println();
    Serial.println("Conectado com sucesso na rede ");
    Serial.print(SSID);
    Serial.println("IP obtido: ");
    Serial.println(WiFi.localIP());

    // Garantir que o LED inicie desligado
    digitalWrite(D4, LOW);
}

// Um callback MQTT é uma função assíncrona invocada automaticamente quando
// um cliente recebe uma mensagem de um broker em um tópico subscrito.
void mqtt_callback(char* topic, byte* payload, unsigned int length) {
    String msg;
    for (int i = 0; i < length; i++) {
        char c = (char)payload[i];
        msg += c;
    }
    Serial.print("- Mensagem recebida: ");
    Serial.println(msg);

    // Forma o padrão de tópico para comparação
    String onTopic = String(topicPrefix) + "@on|";
    String offTopic = String(topicPrefix) + "@off|";
    String rgbTopic = String(topicPrefix) + "@rgb|";

    // Verifica qual o tópico recebido a partir da comparação
    if (msg.equals(onTopic)) {
      digitalWrite(D4, HIGH);
      EstadoSaida = '1';
      myDFPlayer.playFolder(1, 1);
      aguardarAudio();
    }

    if (msg.equals(offTopic)) {
      digitalWrite(D4, LOW);
      EstadoSaida = '0';
      myDFPlayer.playFolder(1, 2);
      aguardarAudio();
    }

    if (msg.startsWith(rgbTopic)) {
        // Extrai apenas a parte dos valores após o "@rgb|"
        // Exemplo: se msg for "lamp001@rgb|255,100,50", rgbValues será "255,100,50"
        String rgbValues = msg.substring(rgbTopic.length());
        
        // Encontra as posições das vírgulas para separar os valores
        int firstCommaIndex = rgbValues.indexOf(',');
        int secondCommaIndex = rgbValues.indexOf(',', firstCommaIndex + 1);
        
        // Valida se encontrou as duas vírgulas (formato correto r,g,b)
        if (firstCommaIndex > 0 && secondCommaIndex > 0) {
          // Separa e converte as strings para inteiros
          int r = rgbValues.substring(0, firstCommaIndex).toInt();
          int g = rgbValues.substring(firstCommaIndex + 1, secondCommaIndex).toInt();
          int b = rgbValues.substring(secondCommaIndex + 1).toInt();
          
          Serial.print("- Comando RGB reconhecido:");
          Serial.print(" R="); Serial.print(r); 
          Serial.print(" G="); Serial.print(g);
          Serial.print(" B="); Serial.println(b);
          
          setRGB(r, g, b, 1.0); // Aplica a cor recebida
          
          // O FIWARE exige que o dispositivo confirme que o comando foi executado
          // Publicamos no tópico cmdexe para atualizar o status da entidade
          String cmdexeTopic = String("/TEF/") + topicPrefix + "/cmdexe";
          String confirmMsg = String("rgb|") + rgbValues;
          MQTT.publish(cmdexeTopic.c_str(), confirmMsg.c_str()); 
        }
      }
}

void VerificaConexoesWiFIEMQTT() {
    if (!MQTT.connected())
        reconnectMQTT();
    reconectWiFi();
}

void EnviaEstadoOutputMQTT() {
    if (EstadoSaida == '1') {
        MQTT.publish(TOPICO_PUBLISH_1, "s|on");
        Serial.println("- Led Ligado");
    }

    if (EstadoSaida == '0') {
        MQTT.publish(TOPICO_PUBLISH_1, "s|off");
        Serial.println("- Led Desligado");
    }
    Serial.println("- Estado do LED onboard enviado ao broker!");
    delay(1000);
}

void InitOutput() {
    // Configura os canais PWM
    ledcAttach(redPin, frequency, resolution);
    ledcAttach(greenPin, frequency, resolution);
    ledcAttach(bluePin, frequency, resolution);

    // Configura e oscila LED embutido
    pinMode(D4, OUTPUT);
    digitalWrite(D4, HIGH);

    boolean toggle = false;

    for (int i = 0; i <= 10; i++) {
        toggle = !toggle;
        digitalWrite(D4, toggle);
        delay(200);
    }
}

void reconnectMQTT() {
    while (!MQTT.connected()) {
        Serial.print("* Tentando se conectar ao Broker MQTT: ");
        Serial.println(BROKER_MQTT);
        if (MQTT.connect(ID_MQTT)) {
          Serial.println("Conectado com sucesso ao broker MQTT!");
          myDFPlayer.playFolder(1, 3);
          aguardarAudio();
          MQTT.subscribe(TOPICO_SUBSCRIBE);
        } else {
            Serial.println("Falha ao reconectar no broker.");
            Serial.println("Haverá nova tentativa de conexão em 2s");
            myDFPlayer.playFolder(1, 4);
            aguardarAudio();
            delay(2000);
        }
    }
}

void handleLuminosity() {
    int sensorValue = analogRead(potPin);
    int luminosity = map(sensorValue, 0, 4095, 100, 0);
    String mensagem = String(luminosity);
    Serial.print("Valor da luminosidade: ");
    Serial.println(mensagem.c_str());
    MQTT.publish(TOPICO_PUBLISH_2, mensagem.c_str());
}

void aguardarAudio() {
  delay(70); // Pequeno delay para o DFPlayer processar o comando e baixar o pino BUSY
  while (digitalRead(busyPin) == LOW) {
    // Enquanto o pino estiver em LOW, o áudio está tocando.
    // Não fazemos nada, apenas esperamos.
    delay(10);
  }
}

void falarBrilho(int valor) {
  if (valor < 0) valor = 0;
  if (valor > 100) valor = 100;

  // 1. Prefixo
  myDFPlayer.playFolder(1, 5);
  aguardarAudio();

  // 2. Lógica dos Números

  if (valor == 0) {
    myDFPlayer.playFolder(3, 1);
    aguardarAudio();
  }
  else if (valor == 100) {
    myDFPlayer.playFolder(3, 21);
    aguardarAudio();
  }
  else if (valor >= 1 && valor <= 19) {
    myDFPlayer.playFolder(3, valor + 1); // "/03/001.mp3 = 'zero'"
    aguardarAudio();
  }
  else if (valor >= 20 && valor <= 99) {
    int dezena = valor / 10;
    int unidade = valor % 10;

    // Fala a dezena:
    myDFPlayer.playFolder(4, dezena - 1); // "/04/001.mp3 = 'vinte'"
    aguardarAudio();

    // Fala a unidade:
    if (unidade > 0) {
      myDFPlayer.playFolder(1, 7); // Conector "e"
      aguardarAudio();

      myDFPlayer.playFolder(3, unidade + 1); // "/03/001.mp3 = 'zero'"
      aguardarAudio();
    }
  }

  // 3. Sufixo
  myDFPlayer.playFolder(1, 6);
  aguardarAudio();
}
