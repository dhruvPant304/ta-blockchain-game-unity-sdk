# TA BlockChain Game Unity SDK
### Content
1. [Introduction](#introduction)
2. [Installation](#installation)
3. [How to use](#how-to-use)
4. [SDK Configs](#sdk-configs)

## Introduction
The TA Blockchain SDK was created to provide a simple and easy way to integrate blockchain functionality into Unity games. The SDK provides a simple interface to interact with the blockchain and provides a simple way to authenticate users using web3Auth. The SDK is designed to be easy to use and provides a simple way to interact with the blockchain without having to worry about the underlying blockchain technology or Back-end architecture.

### Features
- **Web3Auth** - The SDK provides a simple way to authenticate users using web3Auth. The SDK handles all the complexity of web3Auth and provides a simple interface to authenticate users.
- **Leader Boards** - The SDK provides prefabs for leader board UI that can be easily integrated into your game. The SDK handles all the complexity of leader boards and provides a simple interface to interact with the leader boards.
- **User Profile** - The SDK provides prefabs for user profile UI and APIs to fetch profile data that can be easily integrated into your game. The SDK handles all the complexity of user profiles and provides a simple interface to interact with user profiles.
- **Booster Shop and Inventory** - The SDK provides APIs to fetch booster shop data and inventory data that can be easily integrated into your game. The SDK handles all the complexity of booster shop and inventory and provides a simple interface to interact with booster shop and inventory.
- **In App Purchases** - The SDK provides APIs to make in game purchases that can be easily integrated into your game. The SDK handles all the complexity of in game purchases and provides a simple interface to make in game purchases.
- **Push Notifications (Firebase)** - SDK handles device token registration and push notification using **Firebase** handling for your game. Notifications can be configure from backend server.

## Installation
You can install the package using git URL option in Unity package manager windowSDK package, using the following git URL
```
https://github.com/dhruvPant304/ta-blockchain-game-unity-sdk.git?path=Assets/src
```
please note that the SDK package does not include Android plugins that are required for web3Auth as it is observed that these plugins may conflict with existing plugins and cause build errors.
we suggest manually downloading Android plugins and only including the plugins that are not already present in your project.

## How to use

### Getting started using the available samples
We suggest downloading **PlayDoge-Tchi** sample included with the Unity package, from Unity package manager and making a copy of it in your project so you can start making changes to provided prefabs and not have them effected by subsequent package update. Then add the **Landing Page, Login and SignIn** scenes available in PlayDoge-Tchi sample as first,second and third scene in build settings respectively

## SDK Configs