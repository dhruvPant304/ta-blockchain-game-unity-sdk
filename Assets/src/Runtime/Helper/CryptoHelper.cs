using Nethereum.Web3.Accounts;
using Nethereum.Signer;

namespace TA.Crypto{
public static class CryptoHelper {

    public static string GetWalletAddress(string privateKey){
        var account = new Account(privateKey);
        return account.Address;
    }

    public static string GetMessageSignature(string message, string privateKeys){
        var signer = new EthereumMessageSigner();
        var key = new EthECKey(privateKeys);
        var signature = signer.EncodeUTF8AndSign(message, key);
        return signature;
    }
}}
