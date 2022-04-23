using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Bowling_Server.Classes
{
    [Serializable]
    internal class Player
    { 
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("score")]
        public List<int> score { get; set; }

        [NonSerialized]
        public Socket socket;

        public Player(string name)
        {
            this.name = name;
            score = new List<int>();
        }
        public Player()
        {

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
