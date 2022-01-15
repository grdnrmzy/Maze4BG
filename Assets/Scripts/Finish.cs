using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class Finish : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                Logic.Instance.Finish();
        }
    }
}
