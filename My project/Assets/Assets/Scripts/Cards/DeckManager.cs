using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> drawPile = new();
    public List<Card> hand = new();
    public List<Card> discardPile = new();
}
