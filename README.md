# genamon-polygon
Genamon sample project implemented with Polygon

# Overview
This project showcases the ability to generate "Genamon" which are inspired from the Pokemon game boy game. 
The project is powered by Beamable, and will first generate in a Microservice the stats, description and other text data for the genamon.
Once that is complete, it will feed the Genamon description to the Scenario image model to generate image assets at runtime.
Once Scenario has completed the generation, Beamable will publish a notification to the client which will "spawn" the Genamon in the world.

You can capture Genamon, which causes them to be added to the player's Beamable inventory, AND to the Polygon blockchain! 

# Getting Started
You will need a few things before getting started. First, this project depends on the Polygon-Beamable integration package, which requires some setting up.
Please see these instructions first: https://github.com/beamable/polygon-example

You will also need:
- An OpenAI api key using the Davinci model (https://openai.com/)
- A Scenario api key (https://scenario.gg/)
- A trained Scenario model (you can also use a public model)

Once you have obtained the above, edit the following files to input the keys and model id:
- Assets/Beamable/StorageObjects/GenamonStorage/Services/ScenarioGG.cs
- Assets/Beamable/StorageObjects/GenamonStorage/Services/OpenAI.cs

Running the Microservices
1. Make sure you have completed the instructions in the polygon-example project
2. Open the Beamable Microservices Manager
3. Click the "Publish" button to deploy the Microservices to the Cloud

Lastly, you will need to attach a wallet to your Beamable player account before running the DemoGame scene. 
You can do this by running the WalletAuth scene and clicking the Attach button.

# Special Thanks
This project was made possible with free assets from the following talented creators:
- Pixel Art Top Down - Basic (https://assetstore.unity.com/packages/2d/environments/pixel-art-top-down-basic-187605)
- Fantasy Wooden GUI (https://assetstore.unity.com/packages/p/fantasy-wooden-gui-free-103811)
- Cartoon FX Remaster (https://assetstore.unity.com/packages/p/cartoon-fx-remaster-free-109565)
- Particle Effects for UGUI (https://github.com/mob-sakai/ParticleEffectForUGUI)
