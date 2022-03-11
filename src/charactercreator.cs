function cleanupCharacterCreation(%client)
{
	if (isObject(%client.charHolder))
		%client.charHolder.delete();
	if (isObject(%client.charBack))
		%client.charBack.delete();
	if (isObject(%client.charLight))
		%client.charLight.delete();
	if (isObject(%client.charDummy))
		%client.charDummy.delete();
	%client.charModeIndex = "";
	commandToClient(%client, 'ClearCenterPrint');
}

function characterCreationFixPlayer(%client)
{
	%player = %client.player;
	if (isObject(%player) && isObject(%client.camera))
	{
		%client.camera.setMode("Player", %player);
		%client.camera.setControlObject(%client);
		%client.setControlObject(%player);
		
		%client.applyBodyParts();
		commandToClient(%client,'PlayGui_CreateToolHud',%player.getDataBlock().maxTools);
		for (%i = 0; %i <%player.getDataBlock().maxTools; %i++)
		{
			messageClient(%client, 'MsgItemPickup', '', %i, %player.tool[%i], 1);
		}

		if (isEventPending(%client.player.wakeUpSchedule))
			%client.player.setSleepCam();
	}
}

function enterCharacterCreation(%client)
{
	cleanupCharacterCreation(%client);
	
	%x = -5000;
	%y = -5000 + 10000 * getRandom();
	%z = 1000;

	%client.charHolder = new StaticShape()
	{
		datablock = cubeShape;
		position = %x SPC %y SPC %z;
		scale = "10 100 1";
	};
	%client.charHolder.scopeToClient(%client);
	%client.charHolder.setNodeColor("ALL", "0.4 0 0 1");

	%client.charBack = new StaticShape()
	{
		datablock = cubeShape;
		position = %x - 2 SPC %y SPC %z + 2;
		scale = "1 100 10";
	};
	%client.charBack.scopeToClient(%client);
	%client.charBack.setNodeColor("ALL", "0.2 0.2 0.2 1");

	%client.charLight = new fxLight()
	{
		datablock = PlayerLight;
		position = %x + 4 SPC %y SPC %z + 4;
	};
	%client.charLight.scopeToClient(%client);

	%client.charDummy = new Player()
	{
		datablock = PlayerFrozenArmor;
		client = %client;
	};
	%client.charDummy.setShapeNameColor("0.5 1 0.5");

	// case 0: // North
	// 	%aa = "1 0 0 0";
	// case 1: // East
	// 	%aa = "0 0 1 1.57079";
	// case 2: // South
	// 	%aa = "0 0 1 3.14159";
	// case 3: // West
	// 	%aa = "0 0 1 -1.57079";

	%client.charDummy.setTransform(%x SPC %y SPC %z + 1 SPC "0 0 1 0.7853975"); //Northeast
	%client.charDummy.playThread("0", "walk");
	// %client.charDummy.setArmThread("land");
	%client.charDummy.applyAppearance(%client.character);

	//TODO: Port this shit to character.cs
	%client.charDummy.list["skin"] = getSkinColorList();
	%client.charDummy.list["face"] = getFaceList(%client.character.gender);
	%client.charDummy.list["decal"] = getDecalList();
	%client.charDummy.list["hair"] = getHairList(%client.character.gender) TAB getHairList("rare");
	%client.charDummy.list["shirtColor"] = getGenericColorList();
	%client.charDummy.list["pantsColor"] = getPantsColorList();
	%client.charDummy.list["shoesColor"] = getPantsColorList();
	%client.charDummy.list["hairColor"] = getHairColorList() TAB getDyedHairColorList();

	//TODO: Port this shit to character.cs
	%client.charDummy.listToField["skin"] = 0;
	%client.charDummy.listToField["face"] = 1;
	%client.charDummy.listToField["decal"] = 2;
	%client.charDummy.listToField["hair"] = 3;
	%client.charDummy.listToField["shirtColor"] = 4;
	%client.charDummy.listToField["pantsColor"] = 5;
	%client.charDummy.listToField["shoesColor"] = 6;
	%client.charDummy.listToField["hairColor"] = 7;

	%client.charDummy.modes = "skin" TAB "face" TAB "decal" TAB "hair" TAB "shirtColor" TAB "pantsColor" TAB "shoesColor" TAB "hairColor";
	%client.charModeIndex = 0;

	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%client.charIndex = findField(%client.charDummy.list[%charMode], getField(%client.character.appearance, %client.charDummy.listToField[%charMode]));

	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%client.charDummy.list[%charMode], %client.charIndex), "8564862");

	%client.camera.setFlyMode();
	%client.camera.mode = "Observer";
	%client.camera.setTransform(%x + 3 SPC %y SPC %z + 2 @ " 0 0 1 -1.57079"); //West
	%client.camera.setControlObject(%client.dummyCamera);
	%client.setControlObject(%client.camera);

	%client.updateScrollView();

	commandToClient(%client, 'CenterPrint', "<font:arial:20><just:left>\c5Mousewheel  \c7Tool Select<just:right>\c6Scroll \n<just:left><just:left>\c5LMB/RMB  \c7Fire/Jet<just:right>\c6Mode \n<just:left>\c5Space  \c7Jump<just:right>\c6Finish \n<just:left>\c5Shift  \c7Crouch<just:right>\c6Gender ");
}

function characterCreationNext(%client)
{
	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%list = %client.charDummy.list[%charMode];
	%client.charIndex = (%client.charIndex + 1) % getFieldCount(%list);
	%client.character.appearance = setField(%client.character.appearance, %client.charDummy.listToField[%charMode], getField(%list, %client.charIndex));
	%client.charDummy.applyAppearance(%client.character);
	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%list, %client.charIndex), "8564862");
}

function characterCreationPrevious(%client)
{
	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%list = %client.charDummy.list[%charMode];
	%client.charIndex = (%client.charIndex + getFieldCount(%list) - 1) % getFieldCount(%list);
	%client.character.appearance = setField(%client.character.appearance, %client.charDummy.listToField[%charMode], getField(%list, %client.charIndex));
	%client.charDummy.applyAppearance(%client.character);
	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%list, %client.charIndex), "8564862");
}

function characterCreationNextMode(%client)
{
	%client.charModeIndex = (%client.charModeIndex + 1) % getFieldCount(%client.charDummy.modes);
	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%client.charIndex = findField(%client.charDummy.list[%charMode], getField(%client.character.appearance, %client.charDummy.listToField[%charMode]));
	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%client.charDummy.list[%charMode], %client.charIndex), "8564862");
}

function characterCreationPreviousMode(%client)
{
	%client.charModeIndex = (%client.charModeIndex + getFieldCount(%client.charDummy.modes) - 1) % getFieldCount(%client.charDummy.modes);
	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%client.charIndex = findField(%client.charDummy.list[%charMode], getField(%client.character.appearance, %client.charDummy.listToField[%charMode]));
	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%client.charDummy.list[%charMode], %client.charIndex), "8564862");
}

function characterCreationGender(%client, %which)
{
	if (%which $= "male")
		%client.character.gender = "male";
	else
		%client.character.gender = "female";
	%newFace = getFaceList(%client.character.gender);
	%newHair = getHairList(%client.character.gender) TAB getHairList("rare");

	%faceName = getField(%client.character.appearance, 1);
	%hairName = getField(%client.character.appearance, 3);

	if (findField(%newFace, %faceName) == -1)
		%client.character.appearance = setField(%client.character.appearance, 1, getField(%newFace, 0));
	if (findField(%newHair, %hairName) == -1)
		%client.character.appearance = setField(%client.character.appearance, 3, getField(%newHair, 0));

	%client.charDummy.list["face"] = %newFace;
	%client.charDummy.list["hair"] = %newHair;
	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%client.charIndex = findField(%client.charDummy.list[%charMode], getField(%client.character.appearance, %client.charDummy.listToField[%charMode]));
	%client.charDummy.applyAppearance(%client.character);
	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%client.charDummy.list[%charMode], %client.charIndex), "8564862");
}

function characterCreationRandomize(%client)
{
	%client.character.appearance = getRandomAppearance(%client.character.gender);
	%charMode = getField(%client.charDummy.modes, %client.charModeIndex);
	%client.charIndex = findField(%client.charDummy.list[%charMode], getField(%client.character.appearance, %client.charDummy.listToField[%charMode]));
	%client.charDummy.applyAppearance(%client.character);
	%client.charDummy.setShapeName(%charMode @ ": " @ getField(%client.charDummy.list[%charMode], %client.charIndex), "8564862");
}

function GameConnection::updateScrollView(%this)
{
	commandToClient(%this,'PlayGui_CreateToolHud',3);
	for(%i=0; %i<3; %i++)
	{
		messageClient(%this, 'MsgItemPickup', '', %i, scrollIcon.getID(), %i != 0);
	}
	commandToClient(%this, 'SetActiveTool', %player.currTool);
}

//Madman scrolling
datablock ItemData(scrollIcon)
{
	shapeFile = "base/data/shapes/empty.dts";
	iconName = $Despair::Path @ "res/shapes/items/icon_scroll";
	uiName = "Scroll";
	isIcon = true;
	image = printGunImage;
};

package DespairCharacterCreator
{
	function serverCmdUseTool(%client, %slot)
	{
		if(%client.charModeIndex !$= "")
		{
			%diff = %client.charLastSlot - %slot;
			//Rollback fix
			if (%diff == 2)
				%diff = %diff - 3;
			else if (%diff == -2)
				%diff = %diff + 3;
			//could probably be coded better but cba
			if (%diff < 0) //scroll down
				characterCreationNext(%client);
			else if (%diff > 0) //scroll up
				characterCreationPrevious(%client);
			%client.charLastSlot = %slot;
			return;
		}
		Parent::serverCmdUseTool(%client, %slot);
	}
	function serverCmdUnUseTool(%client, %slot)
	{
		if(%client.charModeIndex !$= "")
		{
			// %client.stopViewingInventory();
			return;
		}
		Parent::serverCmdUnUseTool(%client, %slot);
	}
	function Observer::onTrigger(%this, %obj, %slot, %state)
	{
		%client = %obj.getControllingClient();

		if(%client.charModeIndex !$= "")
		{
			if(%slot == 0 && %state) //LMB - Next mode
			{
				characterCreationNextMode(%client);
			}
			if(%slot == 4 && %state) //RMB - Previous Mode
			{
                characterCreationPreviousMode(%client);
			}
			if(%slot == 3 && %state) //Crouch - Change Gender
			{
                characterCreationGender(%client, %client.character.gender $= "male" ? "female" : "male");
			}
			if(%slot == 2 && %state) //Jump - Finish
			{
                cleanupCharacterCreation(%client);
                characterCreationFixPlayer(%client);
			}
			return;
		}

		parent::onTrigger(%this, %obj, %slot, %state);
	}
	function GameConnection::setControlObject(%this, %obj)
	{
		if(%this.charModeIndex !$= "" && %obj != %this.camera) //We probably left the editing mode for some reason.
		{
            cleanupCharacterCreation(%this);
            characterCreationFixPlayer(%this);
        }
		parent::setControlObject(%this, %obj);
	}
    function serverCmdLight(%client)
    {
		if(%client.charModeIndex !$= "")
		{
			characterCreationRandomize(%client);
			return;
		}
        Parent::serverCmdLight(%client);
    }
	function MiniGameSO::removeMember($DefaultMiniGame, %client)
	{
        cleanupCharacterCreation(%client);
		characterCreationFixPlayer(%client);
		Parent::removeMember($DefaultMiniGame, %client);
	}
};

activatePackage("DespairCharacterCreator");