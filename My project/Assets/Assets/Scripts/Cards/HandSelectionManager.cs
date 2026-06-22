public class HandSelectionManager
{
    private CardButton selectedCard;

    public void SelectCard(CardButton card)
    {
        if (selectedCard == card)
        {
            ClearSelection();
            return;
        }

        selectedCard?.SetSelected(false);

        selectedCard = card;
        selectedCard?.SetSelected(true);
    }

    public void ClearSelection()
    {
        selectedCard?.SetSelected(false);
        selectedCard = null;
    }
}
