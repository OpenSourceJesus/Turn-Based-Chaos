using System;
using UnityEngine;
using UnityEngine.Purchasing;
using GridGame;

public class PaymentManager : SingletonMonoBehaviour<PaymentManager>, IStoreListener
{
	public Purchasable[] purchasables = new Purchasable[0];
	public static IStoreController storeController;
	public static IExtensionProvider storeExtensionProvider;
	public static bool IsInitialized
	{
		get
		{
			return storeController != null && storeExtensionProvider != null;;
		}
	}

	public override void Awake ()
	{
		base.Awake ();
		GameManager.singletons.Remove(GetType());
		GameManager.singletons.Add(GetType(), this);
		if (storeController == null)
			InitializePurchasing ();
		gameObject.SetActive(false);
	}

	public void ShowUI ()
	{
		gameObject.SetActive(true);
	}

	public void HideUI ()
	{
		gameObject.SetActive(false);
	}

	public void InitializePurchasing () 
	{
		if (IsInitialized)
			return;
		ConfigurationBuilder configBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		foreach (Purchasable purchasable in purchasables)
			configBuilder.AddProduct(purchasable.id, purchasable.type);
		UnityPurchasing.Initialize(this, configBuilder);
	}

	public void BuyPurchasable (string id)
	{
		if (!IsInitialized)
		{
			print("PaymentManager is not initialized");
			return;
		}
		Product product = storeController.products.WithID(id);
		if (product != null && product.availableToPurchase)
		{
			print("Purchasing: " + product.definition.id);
			storeController.InitiatePurchase(product);
		}
		else
			print("Error purchasing: " + product.definition.id);
	}

	public void RestorePurchases ()
	{
		if (!IsInitialized)
		{
			print("PaymentManager is not initialized");
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			IAppleExtensions apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
			apple.RestoreTransactions(OnRestorePurchases);
		}
		else
			print("Not supported on this platform");
	}
	
	void OnRestorePurchases (bool result)
	{
		print("Continued restoring purchases: " + result + ". If no further messages, no purchases available to restore.");
	}

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		print("PaymentManager initialized");
		storeController = controller;
		storeExtensionProvider = extensions;
	}


	public void OnInitializeFailed (InitializationFailureReason error)
	{
		print("PaymentManager initialization failed because: " + error);
	}


	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args) 
	{
		print("Purchased: " + args.purchasedProduct.definition.id);
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
	{
		print("Purchase of " + product.definition.storeSpecificId + " failed because: " + failureReason);
	}

	[Serializable]
	public class Purchasable
	{
		public string id;
		public ProductType type;
	}
}