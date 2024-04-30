using System;
using System.Collections.Generic;
using System.Linq;

public class RiskCardDeck
{
    private List<RiskCard> cards;

    public RiskCardDeck()
    {
        InitializeDeck();
        Shuffle();
    }

    private void InitializeDeck()
    {
        cards = new List<RiskCard>();
        cards.Add(new RiskCard(RiskCardType.Artillery, 1));
        cards.Add(new RiskCard(RiskCardType.Infantry, 2));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 3));
        cards.Add(new RiskCard(RiskCardType.Infantry, 4));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 5));
        cards.Add(new RiskCard(RiskCardType.Artillery, 6));
        cards.Add(new RiskCard(RiskCardType.Infantry, 7));
        cards.Add(new RiskCard(RiskCardType.Artillery, 8));
        cards.Add(new RiskCard(RiskCardType.Infantry, 9));
        cards.Add(new RiskCard(RiskCardType.Artillery, 10));
        cards.Add(new RiskCard(RiskCardType.Infantry, 11));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 12));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 13));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 14));
        cards.Add(new RiskCard(RiskCardType.Artillery, 15));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 16));
        cards.Add(new RiskCard(RiskCardType.Infantry, 17));
        cards.Add(new RiskCard(RiskCardType.Artillery, 18));
        cards.Add(new RiskCard(RiskCardType.Infantry, 19));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 20));
        cards.Add(new RiskCard(RiskCardType.Artillery, 21));
        cards.Add(new RiskCard(RiskCardType.Artillery, 22));
        cards.Add(new RiskCard(RiskCardType.Infantry, 23));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 24));
        cards.Add(new RiskCard(RiskCardType.Infantry, 25));
        cards.Add(new RiskCard(RiskCardType.Infantry, 26));
        cards.Add(new RiskCard(RiskCardType.Artillery, 27));
        cards.Add(new RiskCard(RiskCardType.Infantry, 28));
        cards.Add(new RiskCard(RiskCardType.Artillery, 29));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 30));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 31));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 32));
        cards.Add(new RiskCard(RiskCardType.Artillery, 33));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 34));
        cards.Add(new RiskCard(RiskCardType.Artillery, 35));
        cards.Add(new RiskCard(RiskCardType.Infantry, 36));
        cards.Add(new RiskCard(RiskCardType.Infantry, 37));
        cards.Add(new RiskCard(RiskCardType.Infantry, 38));
        cards.Add(new RiskCard(RiskCardType.Artillery, 39));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 40));
        cards.Add(new RiskCard(RiskCardType.Artillery, 41));
        cards.Add(new RiskCard(RiskCardType.Cavalry, 42));


    }

    public void Shuffle()
    {
        Random rng = new Random();
        cards = cards.OrderBy(_ => rng.Next()).ToList();
    }

    public RiskCard drawCard()
    {
        if (cards.Count == 0)
        {
            throw new InvalidOperationException("The deck is empty!");
        }

        RiskCard card = cards.First();
        cards.Remove(card);
        return card;
    }
}