using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InAppPurchasesScene : MonoBehaviour {

	public Text label;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void clickedBuyItem1() {
		
		buyItem("sku1",0.99);
	}

	public void clickedBuyItem2() {

		buyItem("sku2",1.99);
	}

	protected void buyItem(string sku, double price) {

		label.text = "";

		UserHook.Instance.updatePurchasedItems(sku, price, new UserHookResponseHandler (this.gameObject.name, "afterUpdateItem"));
	}

	public void afterUpdateItem() {

		label.text = "Item was purchased";
	}

	public void clickedBack ()
	{
		Debug.Log("clicked back");
		SceneManager.LoadScene ("DemoScene");
	}

}
