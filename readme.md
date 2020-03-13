## 1. Introduction
This package provides a framework for the creation of idle/incremental games in Unity.

## 2. Quickstart
Here are some simple instructions to get you started.

To add to your Unity project, download the zip of the project, unzip it and add the contents to your project in a single Folder.

First, you must generate the configuration your engine instance will use:

* Create an instance of IdleFramework.GameConfigurationBuilder and configure it using the following methods:
	* AddResourceDefinition to add definitions of resources you can spend.
	* AddProducerDefinition to add definitions of things that produce resources.

After you are done with the builder, call Build to get a GameConfiguration.

Now, instantiate an IdleFramework.IdleEngine with your configuration; now your engine is ready.

To advance time within the engine, call the Update method on the instance. You are responsible for calling Update at the rate you wish to advance, with the caveat that the engine will not process calls that occur less than 100ms from the last.

To unconditionally change the amount of a resource, call ChangeEntityQuantity on your IdleEngine instance. ChangeEntityQuantity adds or subtracts the given quantity from the current quantity.

To unconditionally set the amount of a resource, call SetResourceQuantity with the key of the entity and new quantity. This discards the previous quantity and unconditionally set it to the new amount.

To buy more of a resource while spending resources, call BuyEntity, with the key of the entity and quantity to attempt to buy. Buying differs from Change and Set in that it checks for if the costs and requirements are met.

## 3. Entities


## 4. Future Features
Engine hooks - This will allow custom user-defined code that can be run when things occur inside the engine that is more sophisticated than what can be done declaratively.
	- Entity purchase
	- Entity production input
	- Entity production output
Tutorial system
Minimum and maximum quantities - Set caps and floors on entity quantities
Tracking of property modifiers - For e.g. tooltips
Custom properties - Add custom properties to the engine and entities.
Read-only views of entity, engine state - So state can be queries without allowing for modification.
Change values to per-second and enable scaling - Allows setting values for e.g. production independent of the update rate.
Unpurchaseable entities - Support entities that cannot be purchased, only produced by e.g. other entities.
Actions which occur over a period of time.
Singleton entities - For things like upgrades/research, characters, etc.