using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Boomlagoon.JSON;
using System;

/* UserSingleton클래스는 현재 유저의 개인 정보 및 능력치 정보를 메모리 상에 들고 있는 싱글톤 클래스 입니다.
   서버로부터 /User/{유저아이디} API로 정보를 가져와서 여기에 저장합니다.
 */

 using Facebook;
 using Facebook.Unity;
public class UserSingleton : MonoBehaviour {

	// UserID입니다. 서버 상에서 유저를 식별하는 고유번호입니다.
	public int UserID{
		get{
			return PlayerPrefs.GetInt("UserID");
		}
		set{
			PlayerPrefs.SetInt("UserID", value);
		}
	}

	// AccessToken은 서버 API에 접근하기 위한 API의 역할을 합니다.
	public string AccessToken{
		get{
			return PlayerPrefs.GetString("AccessToken");
		}
		set{
			PlayerPrefs.SetString("AccessToken", value);
		}
	}

	// 페이스북 아이디입니다. 페이스북의 고유번호입니다. App Scoped User ID입니다.
	public string FacebookID{
		get{
			return PlayerPrefs.GetString("FacebookID");
		}
		set{
			PlayerPrefs.SetString("FacebookID", value);
		}
	}

	// 페이스북에 인증할 수 있는 유저의 개인 비밀번호 키입니다.
	public string FacebookAccessToken{
		get{
			return PlayerPrefs.GetString("FacebookAccessToken");
		}
		set{
			PlayerPrefs.SetString("FacebookAccessToken", value);
		}
	}

	// 유저의 이름입니다. 기본적으로 페이스북의 이름을 가져와 적용합니다.
	public string Name{
		get{
			return PlayerPrefs.GetString("Name");
		}
		set{
			PlayerPrefs.SetString("Name", value);
		}
	}

	// 페이스북의 프로필 사진 주소입니다.
	public string FacebookPhotoURL{
		get{
			return PlayerPrefs.GetString("FacebookPhotoURL");
		}
		set{
			PlayerPrefs.SetString("FacebookPhotoURL", value);
		}
	}

	// 유저의 레벨, 경험치, 체력, 방어력, 스피드, 데미지 레벨, 체력 레벨, 방어력 레벨, 스피드 레벨입니다.
	// 다음 레벨까지 남은 경험치, 그리고 다음 레벨로 레벨업하기 위해 필요한 경험치 정보도 가지고 있습니다.
	public int
		Level, Experience, Damage, Health, Defence, Speed,
		DamageLevel, HealthLevel, DefenceLevel, SpeedLevel, 
		Diamond, ExpAfterLastLevel, ExpForNextLevel;

	public JSONArray FriendList;
	static UserSingleton _instance;
	public static UserSingleton Instance{
		get{
			if(!_instance){
				GameObject container = new GameObject("UserSingleton");
				_instance = container.AddComponent(typeof(UserSingleton)) as UserSingleton;
				DontDestroyOnLoad(container);
			}
			return _instance;
		}
	}

	public void FacebookLogin(Action<bool, string> callback, int retryCount = 0)
	{
		CallFBLogin(callback);
	}
	 private void CallFBLogin(Action<bool, string> callback, int retryCount=0)
	 {
		//FB.LogInWithReadPermissions(new List<string>() { "public_profile", "user_friends" }, this.HandleResult);
		 FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, delegate (ILoginResult result){
			
			if(result.Error != null && retryCount >= 3){
				Debug.LogError("Auth Error : " + result.Error);
				callback(false, result.Error);
				return;
			}
			
			if(result.Error != null){
				Debug.LogError("Auth Error : " + result.Error);
				retryCount = retryCount + 1;
				CallFBLogin(callback, retryCount);
				return;
			}
			else{
				if(FB.IsLoggedIn){

                     Debug.Log("FB Login Result : " + result.ToString());
					
					 // 페이스북 로그인 결과를 JSON 파싱합니다.
					 var aToken = Facebook.Unity.AccessToken.CurrentAccessToken; 
                     Debug.Log(aToken.UserId);

                     // 페이스북 기본 정보들을 UserSingleton에 저장합니다.
                     UserSingleton.Instance.FacebookID = aToken.UserId;
                     UserSingleton.Instance.FacebookAccessToken = aToken.TokenString;
                     UserSingleton.Instance.FacebookID = "http://graph.facebook.com/" +
                        UserSingleton.Instance.FacebookID + "/picture?type=square";

                     //Debug.Log("UserSingleton.Instance.FacebookAccessToken : " + UserSingleton.Instance.FacebookAccessToken);
                     //Debug.Log("UserSingleton.Instance.FacebookID : " + UserSingleton.Instance.FacebookID);
                     //Debug.Log("UserSingleton.Instance.FacebookPhotoURL : " + UserSingleton.Instance.FacebookPhotoURL);

                     callback(true, result.ToString());
				} else {
					Debug.Log("User canceled");
				}
			}
		 });
	 }


    public void LoadFacebookMe(Action<bool, string> callback, int retryCount=0)
    {
        //FB.API("/me", HttpMethod.GET, this.HandleResult);
        FB.API("/me", HttpMethod.GET, delegate(IGraphResult result) {

            if (result.Error != null && retryCount >= 3){
                Debug.LogError(result.Error);
                callback(false, result.Error);
                return;
            }

            if(result.Error != null){
                Debug.LogError("Error occured. start retrying. " + result.Error);
                retryCount = retryCount + 1;
                LoadFacebookMe(callback, retryCount);
                return;
            }

            Debug.Log(result.RawResult);
            JSONObject meObj = JSONObject.Parse(result.RawResult);
            UserSingleton.Instance.Name = meObj["name"].Str;
            callback(true, result.RawResult);
        });
    }

	public void LoadFacebookFriend(Action<bool, string> callback, int retryCount = 0)
    {
        FB.API("/me/friends", HttpMethod.GET, delegate (IGraphResult result) {

            if (result.Error != null && retryCount >= 3)
            {
                Debug.LogError(result.Error);
                callback(false, result.Error);
                return;
            }

            if (result.Error != null)
            {
                Debug.LogError("Error occured. start retrying. " + result.Error);
                retryCount = retryCount + 1;
                LoadFacebookFriend(callback, retryCount);
                return;
            }

            Debug.Log(result.RawResult);
            JSONObject reponseObj = JSONObject.Parse(result.RawResult);
            JSONArray array = reponseObj["data"].Array;
            UserSingleton.Instance.FriendList = array;
            callback(true, result.RawResult);
        });
    }

    public void Refresh(Action callback)
    {
        HTTPClient.Instance.GET(Singleton.Instance.HOST + "/User/Info?UserID=" + UserSingleton.Instance.UserID,
                                delegate (WWW www)
                                {
                                    Debug.Log(www.text);
                                    JSONObject response = JSONObject.Parse(www.text);
                                    int ResultCode = (int)response["ResultCode"].Number;
                                    JSONObject data = response["Data"].Obj;
                                    UserSingleton.Instance.Level = (int)data["Level"].Number;
                                    UserSingleton.Instance.Experience = (int)data["Experience"].Number;
                                    UserSingleton.Instance.Damage = (int)data["Damage"].Number;
                                    UserSingleton.Instance.Health = (int)data["Health"].Number;
                                    UserSingleton.Instance.Defence = (int)data["Defence"].Number;
                                    UserSingleton.Instance.Speed = (int)data["Speed"].Number;
                                    UserSingleton.Instance.DamageLevel = (int)data["DamageLevel"].Number;
                                    UserSingleton.Instance.HealthLevel = (int)data["HealthLevel"].Number;
                                    UserSingleton.Instance.DefenceLevel = (int)data["DefenceLevel"].Number;
                                    UserSingleton.Instance.SpeedLevel = (int)data["SpeedLevel"].Number;
                                    UserSingleton.Instance.Diamond = (int)data["Diamond"].Number;
                                    UserSingleton.Instance.ExpForNextLevel = (int)data["ExpForNextLevel"].Number;
                                    UserSingleton.Instance.ExpAfterLastLevel = (int)data["ExpAfterLastLevel"].Number;
                                });
        
        callback();
    }
	
}
