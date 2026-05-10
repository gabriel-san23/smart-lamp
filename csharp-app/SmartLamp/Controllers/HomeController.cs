using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Teste_SmartLamp.Models;
using System.Text;
using System.Text.Json;

namespace Teste_SmartLamp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View(new FiwareViewModel());
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		//Monta o HTTP CLIENT com headers que o FIWARE precisa
		private HttpClient CriaClienteFiware()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("fiware-service", "smart");
			client.DefaultRequestHeaders.Add("fiware-servicepath", "/");
			client.Timeout = TimeSpan.FromSeconds(10);
			return client;
		}

		//1.1 Health Check - GET
		public async Task<IActionResult> HealthCheckIoTAgent(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:4041/iot/about";
					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//2 Provisionar Grupo de Serviço - POST
		public async Task<IActionResult> ProvisionarGrupo(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:4041/iot/services";

					var body = new
					{
						services = new[]
						{
							new
							{
								apikey = "TEF",
								cbroker = $"http://{serverIp}:1026",
								entity_type = "Thing",
								resource = ""
							}
						}
					};

					string json = JsonSerializer.Serialize(body);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					var response = await client.PostAsync(url, content);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//2.1 Listar Serviços - GET
        public async Task<IActionResult> ListarServicos(string serverIp)
        {
            try
            {
                using (var client = CriaClienteFiware())
                {
                    string url = $"http://{serverIp}:4041/iot/services";
                    var response = await client.GetAsync(url);
                    string corpo = await response.Content.ReadAsStringAsync();
                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
            }
            catch (Exception erro)
            {
                return Json(new { sucesso = false, dados = erro.Message });
            }
        }

        //2.1 Delete Service - DELETE
        public async Task<IActionResult> DeletarGrupo(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:4041/iot/services/?resource=&apikey=TEF";
					var response = await client.DeleteAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//3 Provisionar uma Smart Lamp - POST
		public async Task<IActionResult> ProvisionarLampada(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:4041/iot/devices";

					var body = new
					{
						devices = new[]
						{
							new
							{
								device_id = "lamp001",
								entity_name = "urn:ngsi-ld:Lamp:001",
								entity_type = "Lamp",
								protocol    = "PDI-IoTA-UltraLight",
								transport   = "MQTT",
								commands = new[]
								{
									new { name = "on",  type = "command" },
									new { name = "off", type = "command" },
									new { name = "rgb", type = "command" }
								},
								attributes = new[]
								{
									new { object_id = "s", name = "state",      type = "Text"    },
									new { object_id = "l", name = "luminosity", type = "Integer" }
								}
							}
						}
					};

					string json = JsonSerializer.Serialize(body);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					var response = await client.PostAsync(url, content);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//4 Registrar Comandos - POST
		public async Task<IActionResult> RegistrarComandos(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/registrations";

					var body = new
					{
						description = "Lamp Commands",
						dataProvided = new
						{
							entities = new[]
							{
								new { id = "urn:ngsi-ld:Lamp:001", type = "Lamp" }
							},
							attrs = new[] { "on", "off", "rgb" }
						},
						provider = new
						{
							http = new { url = $"http://{serverIp}:4041" },
							legacyForwarding = true
						}
					};

					string json = JsonSerializer.Serialize(body);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					var response = await client.PostAsync(url, content);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//5 Listar Dispositivos - GET
		public async Task<IActionResult> ListarDispositivos(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:4041/iot/devices";
					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//6 Ligar Lâmpada - PATCH
		public async Task<IActionResult> LigarLampada(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:Lamp:001/attrs";

					var body = new
					{
						on = new { type = "command", value = "" }
					};

					string json = JsonSerializer.Serialize(body);
					var content = new StringContent(json, Encoding.UTF8, "application/json");

					var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
					{
						Content = content
					};

                    var response = await client.SendAsync(request);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//6 Desligar Lâmpada - PATCH
		public async Task<IActionResult> DesligarLampada(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:Lamp:001/attrs";

					var body = new
					{
						off = new { type = "command", value = "" }
					};

					string json = JsonSerializer.Serialize(body);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
					{
						Content = content
					};

					var response = await client.SendAsync(request);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//RGB - Altera cor - PATCH
		public async Task<IActionResult> AlterarCor(string serverIp, int r = 255, int g = 255, int b = 255)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:Lamp:001/attrs";
					string valorRgb = $"{r},{g},{b}";

                    var body = new
                    {
                        rgb = new
                        {
                            type = "command",
                            value = valorRgb
                        }
                    };

                    string json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                    {
                        Content = content
                    };

                    var response = await client.SendAsync(request);
                    string corpo = await response.Content.ReadAsStringAsync();

                    return Json(new
                    {
                        sucesso = response.IsSuccessStatusCode,
                        status = (int)response.StatusCode,
                        dados = corpo
                    });
                }
			}
            catch (Exception erro)
            {
                return Json(new { sucesso = false, dados = erro.Message });
            }
        }

        //7 Resultado de Luminosidade - GET
        public async Task<IActionResult> ObterLuminosidade(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:Lamp:001";
					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//8 Resultado do Estado GET
		public async Task<IActionResult> ObterEstado(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:Lamp:001";
					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//9 Deletar Dispositivo - DELETE
		public async Task<IActionResult> DeletarDispositivoIot(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:4041/iot/devices/lamp001";
					var response = await client.DeleteAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//10 Deletar Entidade do Orion - DELETE
		public async Task<IActionResult> DeletarEntidadeOrion(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/entities/urn:ngsi-ld:Lamp:001?type=Thing";
					var response = await client.DeleteAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//MÉTODOS DO STH COMET

		//1 HealthCheck STH - GET
		public async Task<IActionResult> HealthCheckSth(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:8666/version";
					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//2 Assinar Luminosidade - POST
		public async Task<IActionResult> AssinarLuminosidade(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/v2/subscriptions";

					var body = new
					{
						description = "Notify STH-Comet of all luminosity changes",
						subject = new
						{
							entities = new[]
							{
							new { id = "urn:ngsi-ld:Lamp:001", type = "Lamp" }
						},
							condition = new { attrs = new[] { "luminosity" } }
						},
						notification = new
						{
							http = new { url = $"http://{serverIp}:8666/notify" },
							attrs = new[] { "luminosity" },
							attrsFormat = "legacy"
						}
					};

					string json = JsonSerializer.Serialize(body);
					var content = new StringContent(json, Encoding.UTF8, "application/json");
					var response = await client.PostAsync(url, content);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//3 Obter Histórico - GET
		public async Task<IActionResult> ObterHistorico(string serverIp, int lastN = 30)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:8666/STH/v1/contextEntities/type/Lamp" +
								 $"/id/urn:ngsi-ld:Lamp:001/attributes/luminosity?lastN={lastN}";

					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}

		//MÉTODOS DO ORION CONTEXT BROKER
		
		//1 Version - GET
		public async Task<IActionResult> HealthCheckOrion(string serverIp)
		{
			try
			{
				using (var client = CriaClienteFiware())
				{
					string url = $"http://{serverIp}:1026/version";
					var response = await client.GetAsync(url);
					string corpo = await response.Content.ReadAsStringAsync();

					return Json(new
					{
						sucesso = response.IsSuccessStatusCode,
						status = (int)response.StatusCode,
						dados = corpo
					});
				}
			}
			catch (Exception erro)
			{
				return Json(new { sucesso = false, dados = erro.Message });
			}
		}
	}
}
