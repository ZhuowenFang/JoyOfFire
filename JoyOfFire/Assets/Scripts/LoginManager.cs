using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager instance;
    public InputField UseridInputField;
    public Button loginButton;
    public string userId;
    public Text wrongText;

    public List<String> userIdList = new List<string>
    {
        "9681e94b-fa77-4d67-9bad-54b1d1a8a297",
      "afa4313b-470b-487a-8fe1-35b37f3e8a59",
      "268e3e11-2207-4536-a3fc-5be79e8fdf9a",
      "57160666-186e-4b12-bb8e-25707d5e5c52",
      "5637bbf0-2d8e-4bc6-91f9-80cd1f3356b0",
      "76cb4d00-7901-4f5a-8cce-66b075b02575",
      "52d027eb-f3fb-4c26-9659-e067b3b723b5",
      "b275864c-e018-46d5-ab41-e16124f11c92",
      "7c55fab8-6d23-449d-9bfa-1c57c2d7c54a",
      "d66b2b34-0aad-48a1-afbd-733ded869fbd",
      "47673e90-7801-41bf-a21b-821ed296b558",
      "50382042-3665-461f-bd73-f59f90b328be",
      "1c6e1007-5fbd-4556-a436-e8e6e6fe9d6e",
      "e810c0e7-a56b-4763-92a1-bbf1c894d75b",
      "323030f0-90b6-4ff7-9770-565a8c37fb56",
      "172f7395-4d07-4518-8c35-6a90917b5016",
      "9059b053-c3ac-4020-91de-df2e3b07eb30",
      "2c60e9f2-0924-4dd7-a63f-e45206fdcde4",
      "b3d00aef-1ef4-4b98-ac78-2327b6553349",
      "a2fa5f8c-1049-45cc-a42e-43c65633560f",
      "c9f05f8a-c521-4caf-b4b1-68491033263e",
      "ebfa635b-80b4-4007-836d-bf0c3857775b",
      "8980edb5-5e80-47df-b720-4610a6bb0d23",
      "64908741-7755-4cc0-ab3f-de3254998a6b",
      "67d6cbdb-3d5b-4ee1-a8a3-719039f10b8c",
      "93cd09d4-3584-4bae-ba13-c9d2057d463c",
      "bfa199c0-ac2f-4d94-aebc-ae7e39cff98f",
      "d04683c2-ece1-4366-a622-8e451fcb95bb",
      "2c78911a-3a45-4f20-b8ec-d87f36a0ec45",
      "d1a78e86-0b56-49d5-a920-13a1283d9ef8",
      "5d69c877-d5d0-4f92-9e94-e89b5d95848a",
      "ad1a80ae-0c9a-4c52-83ca-2e3799d4fe83",
      "8caec1be-f60c-4abc-b558-f46ad0078be8",
      "5bbfd0a4-9c1c-4925-91c9-f0d07717e10f",
      "916641f3-6ff6-4607-a9eb-45d1e85db449",
      "034c3822-82f9-42da-8ff0-be34001ef75a",
      "dcbc4bf9-9c30-41b9-93e5-925bbf711f67",
      "d0ddb0ad-dca2-4bcd-8fd0-916395334c21",
      "159ad13e-aacb-4efa-a61c-b871cb03a7bc",
      "3309ae84-5da5-4ef7-8208-12790e5bc5bd",
      "fed48ac9-1512-46b6-8c92-251eae0a5386",
      "e3f5f7fd-2bfa-428e-b895-13b97188cc83",
      "17a90da6-d9f7-45c7-b0fe-62517984c56f",
      "06d8d736-9d3c-4b3a-b9e8-d0a89ab4fb1a",
      "c88b4003-62c5-4756-832e-6fc7eb481201",
      "a98d91c1-8f78-4df2-baaf-70feb814f6be",
      "83321b47-8047-4d2f-855d-ac8c7a558173",
      "8976727f-543f-456a-872d-e0deb418e894",
      "14a01c66-3706-40e6-b90a-4f7c36bbb91a",
      "e4ac2a77-b597-4ee4-bf9f-51ff30850672",
      "5253b0e9-8f7f-4e6e-9034-806dfb75e26d",
      "316ff0a3-3be0-4842-8f2d-24f566677d34",
      "42b85e67-56b9-40bc-b8c6-f59995398a84",
      "abfe5430-2b86-48e4-8f75-b626e3e1b2e0",
      "9f224e3e-a061-463d-9779-99ad95caddbc",
      "70188826-3cad-4c88-ae2c-08840225ec9f",
      "a3a9960c-4b79-441f-b7d4-00b95b9eddec",
      "aeaa65e2-7f3a-4c45-b8ca-7ced58c42b55",
      "691bf719-4a20-4626-81ad-b5596f1d21bb",
      "2c00046f-d13a-4ec1-8a06-6185706b8cb6",
      "7b183bfa-b07b-4474-9121-5b1ecaab1fd4",
      "39412a21-0b02-42d2-a0fe-d5f060d677a0",
      "b9f7934f-9fc5-44e2-9bc6-998a3d8c2454",
      "48fb8ec1-5065-46c6-b25f-17f4699a68a2",
      "ea1dda19-7410-46c6-8453-7e92317bd8c9",
      "42d625d1-6c36-4fb4-af9f-ebe9959fbe67",
      "915704ac-6ca8-43e6-8b99-bcc66d09d2da",
      "e3422130-e5f8-4203-b572-bf456865574c",
      "e42bf19d-7b75-4810-abc5-204f23b43f17",
      "e4c1e0dd-f4dc-4eef-949e-6984cbcef66c",
      "68ea606b-9d2f-4837-824c-6dae6c6a7292",
      "6961a6f6-6b0f-4a02-b72d-3b7cc23fa474",
      "be601012-8065-4ade-b52c-a9d2944afcd6",
      "68aadd30-3bfd-41b8-8a6f-969e5809bf52",
      "d0dbddd0-b9e0-4433-b6bd-54e2669e8dd4",
      "04d6e276-e3de-4471-b75a-ed01217fdde0",
      "2bd81c1e-b99d-4a79-85d9-433c6a32d2b0",
      "a1fdf7a2-de35-4af3-bdca-bf8658a1cb15",
      "cee690d8-572e-4e7e-94fd-20195c7cd90a",
      "ca663000-7774-4a77-ad76-3ea69983f416",
      "2be37841-e7bb-4df5-ac2d-1a8e8dfad724",
      "a9eaecaa-7b8e-44f2-a9c3-915d1d0f8690",
      "f95fc9b1-a297-4de8-9096-2b87ddc7db87",
      "4b757c70-8fc1-4204-bc39-365f2eb4ff7d",
      "1caccc37-38d6-4cdd-a9c3-83ec07e8b511",
      "6a618e88-c4e6-43b8-b5b7-46987ce4969d",
      "51b7248b-7eb5-40ff-a3c4-641443a2ebd6",
      "68d5229e-34aa-42b8-8299-9a99507d5519",
      "20a46d26-3c56-4c4b-892e-25f690d25119",
      "4821b417-a4e6-4442-ba2b-b63f2bdcdbf6",
      "cbaa333d-3f2d-437f-93f0-27469009f9fc",
      "53018164-61e6-4740-9e1a-d461edd9574e",
      "1be534d1-7f93-4dd6-82a2-aa204ab0d8a3",
      "bd2e54a8-a751-4a50-b521-afc6bbd6829c",
      "648ae0a2-d4d0-4a2b-8920-dff0cd6dfadf",
      "5339b34e-ff4a-4cfb-97bd-dafdf99c6274",
      "2fdd324f-08c4-4688-9d93-0cadcb48db4a",
      "fc2d709e-47ae-4787-b34d-f902761167aa",
      "7b3c7704-f517-4460-940f-c84a4f1cc68b",
      "61aa4e25-cc88-4424-ac04-e89d993bb9ac",
      "16208403-ef7a-4e56-9b76-3e09f193e19c",
      "ece57214-fdc0-400e-b2e9-c36f62c55ed8",
      "a8fdb980-5445-4b21-a300-cd4dd08d5eaf",
      "62b83b1d-766f-46f6-8205-e7cbb49d4e0c",
      "e73d76eb-98f3-4b33-94ff-21f543a314ff",
      "0857353b-0244-4499-9ffb-31e696588af5",
      "6583b12a-1796-4732-acac-d5a99aab9298",
      "551bed07-815e-4db0-bb28-1eddd01aa2a5",
      "33bb7992-a588-42c3-a1f3-dd018fdb122a",
      "19be6bbe-2430-4892-87b0-12dcb2616f8f",
      "8a1f9754-a398-4018-9abc-95a9402f4c18",
      "276d1475-107e-41c5-a027-6b634ae03814",
      "b8a74c29-d5f6-439c-b3fe-9d863f2a4611",
      "32156722-4b36-4b45-bac0-421ab7a410af",
      "2f47ab88-02de-49d0-98f4-8bbf46d8bcdd",
      "507fa0fa-12bd-4232-9be8-7492b92888c5",
      "c2983e38-f326-4726-9bf7-5c1eedb698f5",
      "b7a9353b-9897-4b76-b013-b206b1725988",
      "8f916364-b7f0-43d9-a7db-334605ab0288",
      "f1316e06-c5b1-44b2-8db0-d3750305ca42",
      "e2312e61-a436-4398-bd7d-dcfd18bedc25",
      "7a50a531-6b74-4ce7-a8c1-02d33e2a9813",
      "e640cbb7-05d9-4a5b-92f6-a06717834d30",
      "432f2169-a19a-485d-9ca3-9beccf417e8e",
      "2987c89f-696c-4e54-a63e-47634eea5a0d",
      "25484e4c-68f3-4243-8c7d-cd861cb125fd",
      "30fc29b1-d528-4f60-9dcd-2b6c20c87c7f",
      "7342a4f9-e430-4732-b939-ec45d808c0be",
      "752305c9-496b-4e4a-8126-fee25e7639de",
      "c7a84530-8ca7-4067-ad34-11cfb528abec",
      "deefeca8-79bc-4c38-989a-ff8ba54d4b74",
      "c5c2a268-6cfe-454b-a52c-31879495748e",
      "0dbf443d-c4e1-4e8e-b30b-b34e91124d82",
      "699bd005-8bf6-431d-997f-8310acaa0afd",
      "8551f04f-b140-4e13-b7ff-b6079a2aa1c5",
      "bd4409f8-6016-4359-9710-672f367e088e",
      "24a41fa9-9640-4b90-b0b9-987d2a8b92c1",
      "42b1bb5e-a963-47b0-8d1f-55d1d8475be3",
      "0812cff6-cd7a-430d-916a-ea2619514d7d",
      "fc12161f-4df2-459b-8233-3b306806c380",
      "8a458ce9-f8ee-4082-9658-2564adef84c2",
      "898d608b-b485-4f17-806f-aacf495d4402",
      "cbe2346f-9fd5-4874-aaf5-1b34cb443f79",
      "c0b7a122-6317-4db4-a7f7-ae8cead61f0d",
      "7bdedad2-3ef2-45a4-b78c-340e81d83a59",
      "b89daea6-f463-4953-8873-646eaaedd3ca",
      "02d015e6-61ad-4235-bf46-0fbaeaa5aea7",
      "3ddcb73a-5f87-45ca-aedd-0a0ddf423046",
      "20bc445d-1065-42d6-807e-2ced74f978d0",
      "a978d9d1-25d4-4991-af33-9950a262ff7e",
      "6dd96981-8a72-4166-8caa-a31677da8a61",
      "a0a8cf1c-c7ac-44b4-8650-4dac58c58bcb",
      "c7dcda0c-bf49-4c5c-98d4-fd898e717822",
      "3cce1e0c-f231-4455-b353-648b087bb7d4",
      "ae074cfc-cadc-41e3-a1ae-ce1277971f13",
      "d84ca88e-dc7c-46d8-87c8-9d8803fd4a3c",
      "a87e488a-e81b-4b87-bfe4-f167117db9f9",
      "aaec6359-f4f4-4f15-ace2-4afa20be5f3d",
      "14404055-7d02-4a48-ad6c-772998ea0d19",
      "ef905906-0c88-495b-9da6-48bcc30d69c3",
      "bf1ce685-dbba-4b52-99b3-7a547d4e5436",
      "d9012623-ff93-427e-9695-29f5cef23781",
      "0dc3edeb-7334-46a5-8bb1-a977a9e7e555",
      "9c79f4ab-d23c-4965-84cf-0dc153ecdf13",
      "9947effe-76fe-4036-880d-252bd5cbe6fa",
      "1f355664-0ac0-4bc4-9a0e-1c0ae4bd8b62",
      "f2a2561f-3541-460c-8328-2c6645f3ecad",
      "8c55b2ce-fb7d-4ebd-a3b0-9156068993c4",
      "562b73f4-813b-4d84-8704-46db130a4328",
      "76dab837-453d-4a45-b2b0-08f0a8ccde87",
      "0e939275-995e-497b-a8f1-5d1fcef8e734",
      "70e1814b-1f6b-478d-ba73-f42950ae270c",
      "5bbf7429-2d27-4a31-a65d-ce12a3a90aa3",
      "91f126d9-67d6-454a-9d0a-db33bd81497e",
      "7b5403e3-6146-44e8-9da7-d84e1f3d5c92",
      "9c1fbd59-715f-4127-afaa-5daf06ab2cbe",
      "2fad0743-756b-43f3-b818-fb5b5aecb5ab",
      "f9281d7c-5024-4991-b74e-f32c6a2c1a8a",
      "450eb196-004e-4f79-ae3e-51bf66e70fb0",
      "d4735043-b61b-4f11-a44c-54212ead57a3",
      "c84314b3-4943-4204-a972-50f3d5289c9d",
      "58a498f3-0971-4664-b38d-a316ae663de2",
      "eb5d3b6e-99cc-4638-b67c-6c4f4cbc0054",
      "9764e86b-4368-4b22-b838-1142b55e3802",
      "df24807e-54e4-42ab-949e-0c6afadafb0a",
      "15f29953-6d92-48a4-8a0e-9c923ad8752d",
      "418601c9-64c3-44f2-a6dd-e1e36f2e80ed",
      "f3e3c085-a69d-40e4-a547-9cbc00624f5f",
      "4b0b608d-6b06-4bda-8a39-218cb04fb594",
      "866462ab-6424-444f-9165-ef801606a81b",
      "825d23b5-8a5e-4c41-bcba-0de521a9d9be",
      "8774ec38-0e2f-4ad2-a25a-a33aca5c1e13",
      "9ddfa7c6-b917-43a6-b3d2-18c58c5b212f",
      "2ea5c6ad-1214-47a8-a7ed-e810b95d3eb1",
      "1bd0985a-ee26-40e7-90bb-436b12d95c69",
      "88bbb3c3-df9a-46c9-a370-59f3ba031686",
      "75c4398e-7d9a-4deb-b917-b3274012f36b",
      "eea5724a-301d-48ea-b1a8-2443f679cc99",
      "619c1650-2d69-4e39-992c-d9a44d764ec3"
    };
    
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        APIManager.instance.GetLevelData(
            "1-1",
            onSuccess: (response) =>
            {
                Debug.Log("Level data retrieved successfully: " + response);
            },
            onError: (error) =>
            {
                Debug.LogError($"请求失败：{error}");
            }
        );
    }
    
    void OnLoginButtonClicked()
    {
        userId = UseridInputField.text;
        if (userIdList.Contains(userId))
        {
            Debug.Log("Login successful with User ID: " + userId);
            SceneManager.LoadScene("Main Scene");
            // Proceed to the next scene or perform other actions
        }
        else
        {
            Debug.Log("Invalid User ID. Please try again.");
            wrongText.enabled = true;
        }
    }
}
