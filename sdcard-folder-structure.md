# 🗂️ Guia de Organização do Cartão SD (DFPlayer Mini)

Este documento descreve a estrutura de pastas e arquivos de áudio necessários para o sistema de feedback por voz do projeto.

---

## 📂 Estrutura de Pastas

### 📂 Pasta 01: Sistema
Contém avisos de status, conectividade e prefixos de frases.

| Arquivo | Descrição |
| :--- | :--- |
| `001.mp3` | Lâmpada ligada. |
| `002.mp3` | Lâmpada desligada. |
| `003.mp3` | Conectado ao sistema FIWARE. |
| `004.mp3` | Erro de conexão com a rede. |
| `005.mp3` | O brilho foi ajustado para... (Prefixo) |
| `006.mp3` | ...por cento. (Sufixo) |
| `007.mp3` | e (Conector para números) |
| `008.mp3` | Luminosidade baixa detectada no ambiente. |

---

### 📂 Pasta 02: Cores
Contém os avisos de alteração de cores do sistema RGB.

| Arquivo | Descrição |
| :--- | :--- |
| `001.mp3` | Cor alterada para Vermelho. |
| `002.mp3` | Cor alterada para Verde. |
| `003.mp3` | Cor alterada para Azul. |
| `004.mp3` | Cor alterada para Branco. |
| `005.mp3` | Cor alterada para Amarelo. |
| `006.mp3` | Cor alterada para Violeta. |
| `007.mp3` | Cor alterada para Laranja. |

---

### 📂 Pasta 03: Números Base
Contém unidades e números até 19, além do valor máximo.

| Arquivo | Descrição |
| :--- | :--- |
| `001.mp3` - `020.mp3` | Números de **"Zero"** até **"Dezenove"** (em ordem). |
| `021.mp3` | Cem (Para brilho em 100%). |

---

### 📂 Pasta 04: Dezenas
Contém as dezenas exatas para composição de números (ex: 20 + e + 5).

| Arquivo | Descrição |
| :--- | :--- |
| `001.mp3` | Vinte |
| `002.mp3` | Trinta |
| `003.mp3` | Quarenta |
| `004.mp3` | Cinquenta |
| `005.mp3` | Sessenta |
| `006.mp3` | Setenta |
| `007.mp3` | Oitenta |
| `008.mp3` | Noventa |

---

## 🛠️ Especificações Técnicas

- **Formato de Arquivo:** MP3 (recomendado).
- **Taxa de Amostragem:** Idealmente 44.1kHz.
- **Nomenclatura:** Os arquivos devem ser nomeados com 3 dígitos (ex: `001.mp3`) para garantir a compatibilidade com a biblioteca.

### 💻 Exemplo de Implementação no Código (Arduino)

Para reproduzir os arquivos utilizando a biblioteca `DFRobotDFPlayerMini`, utilize a função `playFolder`:

```cpp
// Exemplo: "Cor alterada para Vermelho"
// Pasta 02, Arquivo 001.mp3
myDFPlayer.playFolder(2, 1); 

// Exemplo: "Lâmpada ligada"
// Pasta 01, Arquivo 001.mp3
myDFPlayer.playFolder(1, 1);
