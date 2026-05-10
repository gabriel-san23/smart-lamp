let carregando = false;

function chamar(action) {
    if (carregando) return;

    carregando = true;

    var ip = document.getElementById("serverIp").value;
    document.getElementById("resultado").innerText = "Aguardando...";

    $.ajax({
        url: "/Home/" + action,
        data: { serverIp: ip },

        success: function (dados) {
            let conteudo;

            try {
                conteudo = JSON.stringify(JSON.parse(dados.dados), null, 2);
            } catch {
                conteudo = dados.dados;
            }

            document.getElementById("resultado").innerText =
                (dados.sucesso ? " SUCESSO" : " ERRO") +
                "\nStatus: " + (dados.status ?? "-") +
                "\n\n" + conteudo;
        },

        error: function () {
            document.getElementById("resultado").innerText = "Erro na chamada AJAX.";
        },

        complete: function () {
            carregando = false;
        }
    });
}

function chamarHistorico() {
    var ip = document.getElementById("serverIp").value;
    var lastN = document.getElementById("lastN").value;

    $.ajax({
        url: "/Home/ObterHistorico",
        data: { serverIp: ip, lastN: lastN },
        success: function (dados) {
            document.getElementById("resultado").innerText =
                "Status: " + dados.status + "\n\n" +
                JSON.stringify(JSON.parse(dados.dados), null, 2);
        },
        error: function () {
            document.getElementById("resultado").innerText = "Erro na chamada AJAX.";
        }
    });
}

function chamarRgb() {
    if (carregando) return;

    carregando = true;

    var ip = document.getElementById("serverIp").value;

    var corValor = document.getElementById("corRgb").value;
    var partes = corValor.split(",");

    var r = parseInt(partes[0]);
    var g = parseInt(partes[1]);
    var b = parseInt(partes[2]);

    document.getElementById("resultado").innerText = "Aguardando...";

    $.ajax({
        url: "/Home/AlterarCor",
        data: { serverIp: ip, r: r, g: g, b: b },

        success: function (dados) {
            let conteudo;
            try {
                conteudo = JSON.stringify(JSON.parse(dados.dados), null, 2);
            } catch {
                conteudo = dados.dados;
            }
            document.getElementById("resultado").innerText =
                (dados.sucesso ? " SUCESSO" : " ERRO") +
                "\nStatus: " + (dados.status ?? "-") +
                "\n\n" + conteudo;
        },

        error: function () {
            document.getElementById("resultado").innerText = "Erro na chamada AJAX.";
        },

        complete: function () {
            carregando = false;
        }
    });
}