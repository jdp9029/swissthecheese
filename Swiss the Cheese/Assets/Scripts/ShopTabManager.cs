using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopTabManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI bigMouseName;

    [SerializeField]
    TextMeshProUGUI bigMouseDescription;

    [SerializeField]
    Image selectedSkinMouse;

    [SerializeField]
    Image tab2Skin;

    [SerializeField]
    List<Image> topAccessory;

    [SerializeField]
    List<Image> bottomAccessory;

    [SerializeField]
    TextMeshProUGUI RewardText;

    [SerializeField]
    GameObject SkinButton;

    [SerializeField]
    GameObject TopAccessoryButton;

    [SerializeField]
    GameObject BottomAccessoryButton;

    [SerializeField]
    TextMeshProUGUI Coins;

    [HideInInspector]
    public DictItem SelectedSkin;

    [HideInInspector]
    public DictItem SelectedTopAccessory;

    [HideInInspector]
    public DictItem SelectedBottomAccessory;

    [HideInInspector]
    MouseSkinLoader msl;

    [HideInInspector]
    DictItem[] topAccessories;

    [HideInInspector]
    DictItem[] bottomAccessories;

    [HideInInspector]
    bool rewardAvailable;

    // Start is called before the first frame update
    void Start()
    {
        msl = FindObjectOfType<MouseSkinLoader>();
        topAccessories = msl.Accessories.Where(i => i.IsHat).ToArray();
        bottomAccessories = msl.Accessories.Where(i => !i.IsHat).ToArray();
        SelectedTopAccessory = msl.EquippedTopAccessory;
        SelectedBottomAccessory = msl.EquippedBottomAccessory;

        for(int i = 0; i < transform.childCount; i++)
	    {
	       var obj = transform.GetChild(i);
	       obj.Find("Hitbox").GetComponent<Button>().onClick.AddListener(delegate
	       {
               obj.SetAsLastSibling();
	       });
	    }
	    SelectSkin("Nibbles");
        transform.GetChild(0).Find("Hitbox").GetComponent<Button>().onClick.Invoke();

        Debug.Log(System.DateTime.Now.Day);
        rewardAvailable = PlayerPrefs.GetInt("LastLogin", int.MinValue) != System.DateTime.Now.Day;
        PlayerPrefs.SetInt("LastLogin", System.DateTime.Now.Day);

        SkinButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (!SelectedSkin.IsUnlocked && msl.Coins >= SelectedSkin.ItemCost)
            {
                msl.UnlockItem(SelectedSkin);
            }
            else if (SelectedSkin.IsUnlocked)
            {
                msl.EquippedSkin = SelectedSkin;
                PlayerPrefs.SetString("EquippedSkin", SelectedSkin.Name);
            }
        });

        TopAccessoryButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (!SelectedTopAccessory.IsUnlocked && msl.Coins >= SelectedTopAccessory.ItemCost)
            {
                msl.UnlockItem(SelectedTopAccessory);
            }
            else if (SelectedTopAccessory.IsUnlocked)
            {
                msl.EquippedTopAccessory = SelectedTopAccessory;
                PlayerPrefs.SetString("EquippedTopAccessory", SelectedTopAccessory.Name);
            }
        });

        BottomAccessoryButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            if (!SelectedBottomAccessory.IsUnlocked && msl.Coins >= SelectedBottomAccessory.ItemCost)
            {
                msl.UnlockItem(SelectedBottomAccessory);
            }
            else if (SelectedBottomAccessory.IsUnlocked)
            {
                msl.EquippedBottomAccessory = SelectedBottomAccessory;

                PlayerPrefs.SetString("EquippedBottomAccessory", SelectedBottomAccessory.Name);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        bigMouseName.text = SelectedSkin.Name;
        bigMouseDescription.text = SelectedSkin.Description;

        if (rewardAvailable)
        {
            RewardText.text = "Claim Reward";
        }
        else
        {
            RewardText.text = "Reward Claimed";
        }

        UpdateButtonText(SkinButton, SelectedSkin, msl.EquippedSkin);
        UpdateButtonText(TopAccessoryButton, SelectedTopAccessory, msl.EquippedTopAccessory);
        UpdateButtonText(BottomAccessoryButton, SelectedBottomAccessory, msl.EquippedBottomAccessory);

        Coins.text = msl.Coins.ToString();

        selectedSkinMouse.sprite = SelectedSkin.Sprite;
        tab2Skin.sprite = msl.EquippedSkin.Sprite;
        
        foreach (var img in topAccessory)
        {
            img.sprite = SelectedTopAccessory.Sprite;
            if (SelectedTopAccessory == topAccessories[0])
            {
                img.color = new Color32(255, 255, 255, 0);
            }
            else
            {
                img.color = new Color32(255, 255, 255, 255);
            }
        }
        
        foreach (var img in bottomAccessory)
        {
            img.sprite = SelectedBottomAccessory.Sprite;
            if (SelectedBottomAccessory == bottomAccessories[0])
            {
                img.color = new Color32(255,255,255, 0);
            }
            else
            {
                img.color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void SelectSkin(string name)
    {
        SelectedSkin = FindObjectOfType<MouseSkinLoader>().Skins.First(i => i.Name == name);
    }

    public void NextTopAccessory()
    {
        SelectedTopAccessory = topAccessories[(Array.IndexOf(topAccessories, SelectedTopAccessory) + 1) % topAccessories.Length];
    }
    
    public void NextBottomAccessory()
    {
        SelectedBottomAccessory = bottomAccessories[(Array.IndexOf(bottomAccessories, SelectedBottomAccessory) + 1) % bottomAccessories.Length];
    }
    
    public void PrevTopAccessory()
    {
        SelectedTopAccessory = topAccessories[(Array.IndexOf(topAccessories, SelectedTopAccessory) + topAccessories.Length - 1) % topAccessories.Length];
    }
    
    public void PrevBottomAccessory()
    {
        SelectedBottomAccessory = bottomAccessories[(Array.IndexOf(bottomAccessories, SelectedBottomAccessory) + bottomAccessories.Length - 1) % bottomAccessories.Length];
    }

    public void ClaimReward()
    {
        if (rewardAvailable)
        {
            rewardAvailable = false;
            msl.Coins += 25;
        }
    }

    private void UpdateButtonText(GameObject button, DictItem item, DictItem mslItem)
    {
        if (!item.IsUnlocked)
        {
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Buy ({item.ItemCost})";
        }
        else if (item != mslItem)
        {
            button.SetActive(true);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Equip";
        }
        else
        {
            button.SetActive(false);
        }
    }
}
