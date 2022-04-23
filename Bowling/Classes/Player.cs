using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using NetLib;

namespace Bowling.Classes
{
    [Serializable]
    internal class Player
    {
        [JsonProperty("name")]
        private string name { get; set; }

        [JsonProperty("score")]
        private List<int> score { get; set; }


        public Player(string name)
        {
            this.name = name;
            score = new List<int>();
        }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Deserialize(string json)
        {
            Player player = JsonConvert.DeserializeObject<Player>(json);
            name = player.name;
            score = player.score;
        }
    }
}
