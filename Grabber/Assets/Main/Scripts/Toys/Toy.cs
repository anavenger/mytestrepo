using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTP.Toys
{
    public abstract class Toy : MonoBehaviour
    {
        [SerializeField] private bool isInBusket;
        [SerializeField] private int score;
        public bool IsInBusket
        {
            get { return isInBusket;}
            set {  isInBusket = value;}
        }

        public int Score => score;
        public bool IsDestroying { get; set; }
    }
}

