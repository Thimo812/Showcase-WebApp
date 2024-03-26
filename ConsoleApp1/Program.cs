
using Newtonsoft.Json;
using Showcase_WebApp.data.DataAccessObjects;
using Showcase_WebApp.Models;

var session = new GameSessionModel(new Player("thimo", "123"), new Player("Timo", "456"), "Thimo", "Balls");

var json = JsonConvert.SerializeObject(session);

Console.WriteLine(json);

