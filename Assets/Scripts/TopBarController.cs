using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class TopBarController : MonoBehaviour {

    public Text txtName;//, txtLevel, txtExp, txtDiamond;
    public RawImage profileImage;

    private void Start()
    {
        UpdatePlayerData();
        
    }

    void UpdatePlayerData()
    {
        txtName.text = UserSingleton.Instance.Name;

        string url = UserSingleton.Instance.FacebookPhotoURL;
        Debug.Log(url);

        HTTPClient.Instance.GET(url, delegate (WWW obj)
        {
            profileImage.texture = obj.texture;
        });
    }
}
