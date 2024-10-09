using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MerchantBuyOptionRefs : MonoBehaviour
{
    public Image collectibleImage;
    public TMP_Text unitPrice;
    public TMP_Text selectedAmount;
    public TMP_Text maxAmount;
    public TMP_Text buyTotal;
    public Button minusButton;
    public Button plusButton;

    public void OnMinusClick()
    {
        // button should be disabled if val is 1 so it will only be clicked if greater than 1

        int currentSelectedAmt = int.Parse(selectedAmount.text);
		int maxAmt = int.Parse(maxAmount.text);

		currentSelectedAmt--;
        if (currentSelectedAmt == 1) minusButton.interactable = false;
        if (maxAmt > currentSelectedAmt) plusButton.interactable = true;
        selectedAmount.text = currentSelectedAmt.ToString();
        UpdateTotal(currentSelectedAmt);
    }

    public void OnPlusClick()
    {
		int currentSelectedAmt = int.Parse(selectedAmount.text);
		int maxAmt = int.Parse(maxAmount.text);
		currentSelectedAmt++;
		if (currentSelectedAmt == 2) minusButton.interactable = true;
        if (currentSelectedAmt == maxAmt) plusButton.interactable = false;
		selectedAmount.text = currentSelectedAmt.ToString();
        UpdateTotal(currentSelectedAmt);
	}

    public void UpdateTotal(int selectedAmt)
    {
        int unitCost = int.Parse(unitPrice.text);
        int total = selectedAmt * unitCost;

		buyTotal.text = total.ToString();
	}
}
