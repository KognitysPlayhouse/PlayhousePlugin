using System.Collections.Generic;
using Exiled.API.Features;
using UnityEngine;

namespace PlayhousePlugin.CustomClass
{
    public class CustomClassManager : MonoBehaviour
    {
        public static Dictionary<Player, CustomClassManager> Players = new Dictionary<Player, CustomClassManager>();
        
        public CustomClassBase CustomClass { get; set; }
        public Player Ply;
        public int AbilityIndex = -1;

        private void Awake()
        {
            Ply = Player.Get(gameObject);
        }

        private void OnDestroy()
        {
            CustomClass?.Dispose();
        }

        public void DisposeCustomClass()
        {
            CustomClass?.Dispose();
            CustomClass = null;
            AbilityIndex = -1;
        }
    }
}