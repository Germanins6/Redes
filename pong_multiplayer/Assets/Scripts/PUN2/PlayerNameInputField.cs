using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using System.Collections;


[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{

        const string playerNamePrefKey = "PlayerName";

        void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();

            //Set name if input field is != null
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }


            PhotonNetwork.NickName = defaultName;
        }




        #region Public Methods



        public void SetPlayerName(string value)
        {
            // #Important
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player Name is null or empty");
                return;
            }
            PhotonNetwork.NickName = value;


            PlayerPrefs.SetString(playerNamePrefKey, value);
        }


        #endregion
}