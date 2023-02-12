using UnityEngine;
using Thirdweb;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RewardsButtons : MonoBehaviour
{
    private ThirdwebSDK sdk;
    private string address;
    private string contractAddress = "0xB6d2b657A7f7d05587bCa1647B263Ab54f456263";

    void Start()
    {
        sdk = new ThirdwebSDK("goerli");
    }

    void Update()
    {

    }
    public async void ConnectWallet()
    {
        // Connect to the wallet
        address = await sdk.wallet.Connect();
    }

    public async void GetNFT()
    {

        var contract = sdk.GetContract(contractAddress);

        var quantity = 1; // how many unique NFTs you want to claim

        var tx = await contract.ERC721.ClaimTo(address, quantity);
    }
}
