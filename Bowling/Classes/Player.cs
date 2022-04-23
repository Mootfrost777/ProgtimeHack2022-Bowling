using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;  

namespace Bowling.Classes
{
    [Serializable]
    internal class Player
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public List<int> Score { get; set; }

        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Deserialize(string json)
        {
            Player player = JsonConvert.DeserializeObject<Player>(json);
            Name = player.Name;
            Score = player.Score;
        }
    }
}
