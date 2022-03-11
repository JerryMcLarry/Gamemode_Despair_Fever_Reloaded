datablock TSShapeConstructor(mDespairDts) {
	baseShape = "base/data/shapes/player/m_df.dts";
	sequence0 = "base/data/shapes/player/default.dsq";
	sequence1 = "base/data/shapes/player/player_anim_1h.dsq";
	sequence2 = "base/data/shapes/player/player_anim_misc.dsq";
};

datablock PlayerData(PlayerDespairArmor : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/m_df.dts";
	uiName = "Despair Fever Player";

	cameraMaxDist = 2;
	cameraVerticalOffset = 1.25;
	maxFreelookAngle = 2;

	canJet = 0;
	mass = 120;

	runForce = 6000;

	maxForwardSpeed = 8;
	maxBackwardSpeed = 5;
	maxSideSpeed = 7;

	maxEnergy = 100;

	minImpactSpeed = 20;

	jumpForce = 1200;
	jumpDelay = 30;

	jumpEnergyDrain = 50;
	minJumpEnergy = 75;

	showEnergyBar = 1;

	airControl = 0.05;

	jumpSound = "";

	maxTools = 6;
};

function PlayerDespairArmor::onAdd(%data, %player)
{
	parent::onAdd(%data, %player);
	if(%player.maxhealth $= "")
		%player.maxhealth = 100;
	if(%player.health $= "")
		%player.health = 100;
	if(%player.swingSpeedMod $= "")
		%player.swingSpeedMod = 1;
	if(%player.speedScale $= "")
		%player.speedScale = 1;
	if(%player.mood $= "")
		%player.setMood(0, "Your mood is neutral.");
	if(!isEventPending(%player.traitSchedule))
		%player.traitSchedule();
	if(!isEventPending(%player.moodSchedule))
		%player.moodSchedule();
}

datablock PlayerData(PlayerCorpseArmor : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/m_df.dts";
	uiName = "Corpse Player";

	firstPersonOnly = true;

	cameraMaxDist = 2;
	cameraVerticalOffset = 1.25;
	maxFreelookAngle = 2;

	boundingBox = "5 5 4";
	crouchBoundingBox = "5 5 4";

	canJet = 0;
	mass = 120;

	maxForwardSpeed = 0;
	maxBackwardSpeed = 1;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 2;
	maxSideCrouchSpeed = 0;

	minImpactSpeed = 14;
	speedDamageScale = 15;

	jumpForce = 0;

	maxTools = 6;

	disableFootsteps = true;
};

datablock PlayerData(PlayerFrozenArmor : PlayerStandardArmor)
{
	shapeFile = "base/data/shapes/player/m_df.dts";
	uiName = "Frozen Player";

	crouchBoundingBox = "5 5 10.6"; //no dodging votes fucko

	canJet = 0;
	mass = 120;

	maxForwardSpeed = 0;
	maxBackwardSpeed = 0;
	maxSideSpeed = 0;

	maxForwardCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxSideCrouchSpeed = 0;

	jumpForce = 0;

	maxTools = 6;

	disableFootsteps = true;
};

function PlayerDespairArmor::doDismount(%this, %obj)
{
	if(%obj.getObjectMount().getDatablock() == nameToID(DespairStand))
		return;
	parent::doDismount(%this, %obj);
}

function PlayerDespairArmor::onUnMount(%this, %obj, %mount, %slot)
{
	Parent::onUnMount(%this, %obj, %mount, %slot);
	if (!isObject(%mount.heldCorpse) || %mount.heldCorpse != %obj)
		return;
	%mount.heldCorpse = "";
	%mount.playThread(2, "root");
}

function PlayerDespairArmor::killerDash(%this, %obj, %end)
{
	if(%end)
	{
		%obj.updateSpeedScale(1);
		return;
	}
	%obj.setWhiteOut(0.1);
	%obj.setSpeedScale(1.3);
	%this.schedule(1000, killerDash, %obj, 1);
}

function PlayerDespairArmor::onTrigger(%this, %obj, %slot, %state)
{
	if(%obj.isViewingInventory)
		return Parent::onTrigger(%this, %obj, %slot, %state);

	if(%slot == 0 && %state) //pick items up if we don't have any active inventory items selected
	{
		%item = %obj.tool[%obj.currTool];
		if(%item && isFunction(%item.getName(), "onWear"))
		{
			%item.onWear(%obj);
			return;
		}
		else if(!isObject(%obj.getMountedImage(0)))
		{
			%range = 6;
			if($DespairTrial)
				%range = 48;

			%a = %obj.getEyePoint();
			%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), %range));

			%mask =
				$TypeMasks::FxBrickObjectType |
				$TypeMasks::PlayerObjectType |
				$TypeMasks::CorpseObjectType |
				$TypeMasks::StaticShapeObjectType |
				$TypeMasks::TerrainObjectType |
				$TypeMasks::ItemObjectType;

			%ray = containerRayCast(%a, %b, %mask, %obj);
			if(isObject(%ray))
			{
				if(%ray.getClassName() $= "Item")
				{
					if(!%ray.canPickup) //examine
					{
						%obj.client.examineObject(%ray);
						return;
					}
					%data = %ray.getDataBlock();
					if((%data.waitForKiller || %data.disguise) && !$pickedKiller) //can't pickup unless killer is on
					{
						commandToClient(%obj.client, 'CenterPrint', "<font:cambria:24>\c3You cannot pick up" SPC %data.uiName SPC "before your role is decided!", 2);
						return;
					}
					if(%data.className $= "DespairWeapon")
					{
						if(%obj.tool[%obj.weaponSlot] == nameToID(noWeaponIcon))
						{
							%obj.setTool(%obj.weaponSlot, %data, %ray.itemProps, 1, 0);
							%ray.itemProps = "";
							%ray.delete();
						}
						return;
					}
					if(%data.className $= "Hat")
					{
						%data.onPickup(%ray, %obj);
						return;
					}
					if(%data.getName() $= "PaperstackItem")
					{
						%data.onPickUp(%ray, %obj);
						return;
					}
					if (%ray.canPickup && (%slot = %obj.addTool(%data, %ray.itemProps, 1, 0)) != -1)
					{
						if(%data.getName() $= "MopItem" || %data.getName() $= "RadioItem")
							%data.onPickup(%ray, %obj, %slot);
						%ray.itemProps = "";
						%ray.delete();
						return;
					}
				}
				else if(%ray.getType() & $TypeMasks::PlayerObjectType && !%ray.unconscious) //corpse.cs handles both carrying and examination of sleeping folks
				{
					if(isObject(%obj.client))
						%obj.client.examineObject(%ray);
				}
			}

			initContainerRadiusSearch(getWords(%ray, 1, 3), 0.1,
				$TypeMasks::StaticShapeObjectType);

			if (isObject(%col = containerSearchNext()))
			{
				if(isObject(%obj.client))
					%obj.client.examineObject(%col);
			}
		}
	}
	Parent::onTrigger(%this, %obj, %slot, %state);
	if(%obj.client.killer && $deathCount <= 0 && %slot == 4 && %state) //No more sprinting if there's already a victim
	{
		if(isObject(%img = %obj.getMountedImage(0)) && %img.item.className !$= "DespairWeapon")
			return;
		if($Sim::Time - %obj.lastKillerDash < 5)
			return;
		%obj.lastKillerDash = $Sim::Time;
		%this.killerDash(%obj);
	}
}

function PlayerFrozenArmor::onTrigger(%this, %obj, %slot, %state)
{
	PlayerDespairArmor::onTrigger(%this, %obj, %slot, %state);
}

function Player::dropTool(%obj, %index, %vel)
{
	if($DespairTrial)
	{
		DespairTrialDropTool(%obj, %index);
		return;
	}
	if(%obj.unconscious)
		return;
	%item = %obj.tool[%index];
	if(!isObject(%item) || %item.isIcon)
		return;

	if(%vel $= "")
		%vel = 15;

	%range = 6;

	%a = %obj.getEyePoint();
	%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), %range));

	%mask =
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::PlayerObjectType |
		$TypeMasks::CorpseObjectType |
		$TypeMasks::StaticShapeObjectType |
		$TypeMasks::TerrainObjectType |
		$TypeMasks::ItemObjectType;

	%ray = containerRayCast(%a, %b, %mask, %obj);
	if(isObject(%ray))
	{
		if(%ray.getClassName() $= "fxDTSBrick" && %ray.getName() $= "_furnace")
		{
			if(!%obj.client.killer)
			{
				messageClient(%obj.client, '', "\c5You're too pure for burning things!");
				return;
			}
			if(%item.className $= "DespairWeapon")
			{
				messageClient(%obj.client, '', "\c5This object is too big to burn!");
				return;
			}
			if($Sim::Time - $FurnaceLastUsage > 300)
			{
				$FurnaceLastUsage = $Sim::Time;
				for (%i = 0; %i < BrickGroup_888888.NTObjectCount["_furnace_smoke"]; %i++)
				{
					%brick = BrickGroup_888888.NTObject["_furnace_smoke", %i];
					%brick.setEmitter("FogEmitterA");
					%brick.schedule(300 * 1000, "setEmitter", "");
				}

				if(isObject(%obj.itemProps[%index]))
					%obj.itemProps[%index].delete();
				%obj.itemProps[%index] = "";

				if(isFunction(%item.getName(), "onDrop"))
					%item.onDrop(%obj, %index);
				else
					%obj.removeTool(%index, 1, 0, 0);

				RS_Log(%obj.client.getPlayerName() SPC "(" @ %obj.client.getBLID() @ ") burned '" @ %item.uiName @ "' in the furnace", "\c1");
			}
			else
			{
				messageClient(%obj.client, '', "\c5The furnace is still burning! You'll have to wait.");
			}
			return;
		}
	}

	%spawn = new Item() {
		dataBlock = %item;
		position = %obj.getEyePoint();
		itemProps = %obj.itemProps[%index];
	};
	%spawn.setCollisionTimeout(%obj);
	%spawn.setTransform(getWords(%obj.getEyePoint(), 0, 2) SPC getWords(%obj.getTransform(), 3, 7));
	%targVel = VectorScale(%obj.getEyeVector(), %vel);
	%spawn.setVelocity(VectorAdd(%obj.getVelocity(), %targVel));
	%spawn.schedulePop();
	%spawn.sourceObject = %obj;

	if(isObject(%obj.itemProps[%index]))
		%obj.itemProps[%index].onOwnerChange(%spawn);
	%obj.itemProps[%index] = "";

	if(isFunction(%item.getName(), "onDrop"))
		%item.onDrop(%obj, %index);
	else
		%obj.removeTool(%index, 1, 0, 0);

	RS_Log(%obj.client.getPlayerName() SPC "(" @ %obj.client.getBLID() @ ") used /dropTool '" @ %item.uiName @ "'", "\c1");
}

function Player::setSpeedScale(%obj, %scale)
{
	%db = %obj.getDataBlock();

	%obj.setMaxForwardSpeed(%db.maxForwardSpeed * %scale);
	%obj.setMaxBackwardSpeed(%db.maxBackwardSpeed * %scale);
	%obj.setMaxSideSpeed(%db.maxSideSpeed * %scale);
	
	%obj.setMaxCrouchForwardSpeed(%db.maxForwardCrouchSpeed * %scale);
	%obj.setMaxCrouchBackwardSpeed(%db.maxBackwardCrouchSpeed * %scale);
	%obj.setMaxCrouchSideSpeed(%db.maxSideCrouchSpeed * %scale);
}

function Player::updateSpeedScale(%obj, %int)
{
	%obj.speedScale = %int;
	if(isObject(%obj.character))
	{
		if (%obj.character.trait["Athletic"])
			%obj.speedScale += 0.1;
		else if (%obj.character.trait["Sluggish"])
			%obj.speedScale -= 0.1;
	}

	if (getMoodName(%obj.mood) $= "overjoyed")
		%obj.speedScale += 0.1;
	else if (getMoodName(%obj.mood) $= "depressed")
		%obj.speedScale -= 0.1;

	%obj.setSpeedScale(getMin(%obj.speedScale, 1.2));
}

function Player::onLight(%this)
{
	if(!isObject(%client = %this.client))
		return;

	if(isObject(%col = %this.findCorpseRayCast()))
	{
		%this.playThread(2, "activate2");
		if(!%this.isViewingInventory)
			%client.startViewingInventory(%col, %col.getDatablock().maxTools);
		else
			%client.updateInventoryView();
		return;
	}
	%a = %this.getEyePoint();
	%b = vectorAdd(%a, vectorScale(%this.getEyeVector(), 4));

	%mask =
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::PlayerObjectType;

	%ray = containerRayCast(%a, %b, %mask, %this);

	if (%ray && %ray.getDataBlock().isDoor)
	{
		%this.playThread(2, "activate");
		serverPlay3d(DoorKnockSound, %ray.getWorldBoxCenter());
		return;
	}
	if(%client.killer)
	{
		if ($Sim::Time - %client.lastKillerScan < 3)
			return;
		%client.lastKillerScan = $Sim::Time;

		%close = 0;
		for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
		{
			%member = $DefaultMiniGame.member[%i];

			if (%member.player && %member.player != %this)
			{
				%dist = vectorDist(%this.getEyePoint(), %member.player.getEyePoint());
				if (%dist <= 32)
				{
					%client.schedule(500 * %close, play3d, HeartBeatSound, %member.player.getEyePoint());
					%close += 1;
				}
			}
		}
		%client.centerPrint("\c6There are \c3" @ %close @ "\c6 people in talking distance.", 2);
	}
}

function player::IsBloody(%pl)
{
	return %pl.bloody["lshoe"] || %pl.bloody["rshoe"] ||
	%pl.bloody["lhand"] || %pl.bloody["rhand"] ||
	%pl.bloody["head"] || %pl.bloody["headshot"] ||
	%pl.bloody["chest_front"] || %pl.bloody["chest_back"];
}

function player::applyAppearance(%pl,%char)
{
	%pl.hideNode("ALL");
	if(%char $= "")
		%char = %pl.character;
	%app = %char.appearance;
	%female = %char.gender $= "Female";

	%handColor = getField(%app, 0);
	%headColor = %handColor;
	%faceName = getField(%app, 1);
	%decalName = getField(%app, 2);
	%hairName = getField(%app, 3);
	%shirtColor = getField(%app, 4);
	%pantsColor = getField(%app, 5);
	%shoesColor = getField(%app, 6);
	%hairColor = getField(%app, 7);

	if(%pl.desaturate)
	{
		%a = 0.8;
		%handColor = desaturateRGB(%handColor, %a) SPC 1;
		%headColor = desaturateRGB(%headColor, %a) SPC 1;
		%shirtColor = desaturateRGB(%shirtColor, %a) SPC 1;
		%pantsColor = desaturateRGB(%pantsColor, %a) SPC 1;
		%shoesColor = desaturateRGB(%shoesColor, %a) SPC 1;
		%hairColor = desaturateRGB(%hairColor, %a) SPC 1;
	}

	if(%pl.mangled)
	{
		%handColor = 0.75 + 0.1 * getRandom() @ " 0 0 1";
		%headColor = 0.75 + 0.1 * getRandom() @ " 0 0 1";
		%shirtColor = 0.75 + 0.1 * getRandom() @ " 0 0 1";
		%pantsColor = 0.75 + 0.1 * getRandom() @ " 0 0 1";
		%shoesColor = 0.75 + 0.1 * getRandom() @ " 0 0 1";

		%faceName = "asciiTerror";
		%decalName = "";
		%hairName = "";

		%female = true;
	}


	if(isObject(%pl.getMountedImage(1)) && %pl.getMountedImage(1).item.hideAppearance)
	{
		%hideApp = true;
		%shoesColor = "0.25 0.25 0.25 1";
	}
	if(isObject(%hat = %pl.tool[%pl.hatSlot]) && isObject(%pl.getMountedImage(2)) && %pl.getMountedImage(2) == nameToID(%hat.image))
	{
		if(%hairName !$= "" && %hat.replaceHair !$= "")
			%hairName = %hat.replaceHair;
		if(%hairName !$= "" && %hat.replaceHair[%char.gender] !$= "")
			%hairName = %hat.replaceHair[%char.gender];
		if(%hat.hideHair)
			%hairName = "";
		if(%hat.disguise)
		{
			%faceName = "smiley";
			if(%hideApp)
				%handColor = "0.25 0.25 0.25 1";
		}
		if(%hat.nodeColor !$= "")
			%headColor = %hat.nodeColor SPC "1";
	}

	if(!%hideApp)
	{
		%pl.unHideNode((%female ? "femChest" : "chest"));

		%pl.unHideNode((%female ? "rarmSlim" : "rarm"));
		%pl.unHideNode((%female ? "larmSlim" : "larm"));
	}

	%pl.unHideNode((%rhook ? "rhook" : "rhand"));
	%pl.unHideNode((%lhook ? "lhook" : "lhand"));

	%pl.unHideNode("headskin");

	if (%hairName !$= "")
		%pl.unHideNode(%hairName);

	%pl.setHeadUp(false);

	if(%hip && !%hideApp)
	{
		%pl.unHideNode("skirthip");
		%pl.unHideNode("skirttrimleft");
		%pl.unHideNode("skirttrimright");
	}
	else
	{
		if(!%hideApp)
			%pl.unHideNode("pants");
		%pl.unHideNode((%rpeg ? "rpeg" : "rshoe"));
		%pl.unHideNode((%lpeg ? "lpeg" : "lshoe"));
	}

	if (%pl.bloody["lshoe"])
		%pl.unHideNode("lshoe_blood");
	if (%pl.bloody["rshoe"])
		%pl.unHideNode("rshoe_blood");
	if (%pl.bloody["lhand"])
		%pl.unHideNode("lhand_blood");
	if (%pl.bloody["rhand"])
		%pl.unHideNode("rhand_blood");
	if (%pl.bloody["head"])
		%pl.unHideNode("blood_head");
	//if (%pl.bloody["headshot"])
		//%pl.unHideNode("HeadShot");
	if(!%hideApp)
	{
		if (%pl.bloody["chest_front"])
			%pl.unHideNode((%female ? "fem" : "") @ "chest_blood_front");
		if (%pl.bloody["chest_back"])
			%pl.unHideNode((%female ? "fem" : "") @ "chest_blood_back");
	}

	%pl.setFaceName(%faceName);
	%pl.setDecalName(%decalName);
	
	%pl.setNodeColor("headskin", %headColor);
	//hair color
	if(%hairName !$= "")
		%pl.setNodeColor(%hairName, %hairColor);

	%pl.setNodeColor("chest",%shirtColor);
	%pl.setNodeColor("femChest",%shirtColor);
	%pl.setNodeColor("pants",%pantsColor);
	%pl.setNodeColor("skirthip",%pantsColor);
	
	%pl.setNodeColor("rarm",%shirtColor);
	%pl.setNodeColor("larm",%shirtColor);
	%pl.setNodeColor("rarmSlim",%shirtColor);
	%pl.setNodeColor("larmSlim",%shirtColor);
	
	%pl.setNodeColor("rhand",%handColor);
	%pl.setNodeColor("lhand",%handColor);
	%pl.setNodeColor("rhook","0.35 0.35 0.35 1");
	%pl.setNodeColor("lhook","0.35 0.35 0.35 1");
	
	%pl.setNodeColor("rshoe",%shoesColor);
	%pl.setNodeColor("lshoe",%shoesColor);
	%pl.setNodeColor("rpeg","0.47 0.35 0.2 1");
	%pl.setNodeColor("lpeg","0.47 0.35 0.2 1");
	%pl.setNodeColor("skirttrimright",%pantsColor);
	%pl.setNodeColor("skirttrimleft",%pantsColor);

	//Set blood colors.
	%pl.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%pl.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%pl.setNodeColor("lhand_blood", "0.7 0 0 1");
	%pl.setNodeColor("rhand_blood", "0.7 0 0 1");
	%pl.setNodeColor("blood_head", "0.7 0 0 1");
	//%pl.setNodeColor("HeadShot", "0.7 0 0 1");
	%pl.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%pl.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%pl.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%pl.setNodeColor("femchest_blood_back", "0.7 0 0 1");
}

package DespairPlayerPackage
{
	function Player::playThread(%this, %slot, %sequenceName)
	{
		if (%this.getDataBlock().getID() != nameToID(PlayerDespairArmor) || %slot != 3)
		{
			return Parent::playThread(%this, %slot, %sequenceName);
		}

		Parent::playThread(%this, %slot, %sequenceName);

		if (%sequenceName !$= "activate2" || $Sim::Time - %this.lastRapidClick < 0.8)
			return;

		%this.lastRapidClick = $Sim::Time;

		%eyePoint = %this.getEyePoint();
		%eyeVector = %this.getEyeVector();

		%ray = containerRayCast(%eyePoint,
			vectorAdd(%eyePoint, vectorScale(%eyeVector, 5)),
			$TypeMasks::PlayerObjectType |
			$TypeMasks::FxBrickObjectType,
			%this
		);

		if (!%ray)
			return;

		%col = getWord(%ray, 0);

		if (!(%col.getType() & $TypeMasks::PlayerObjectType) || %ray.getDataBlock().isPushImmune)
			return;

		%velocity = vectorScale(%this.getForwardVector(), 8);
		%velocity = vectorAdd(%velocity, vectorScale(%col.getVelocity(), 0.5));
		%velocity = vectorAdd(%velocity, "0 0 5");

		%col.setVelocity(%velocity);
		%col.lastShover = %this;
		%col.lastShoved = $Sim::Time;

		ServerPlay3D("ShoveSound", getWords(%ray, 1, 3));
		if(isObject(%client = %this.client))
			RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") shoved " @ %col.client.getPlayerName() SPC "(" @ %col.client.getBLID() @ ")", "\c1");
	}
	function Observer::onTrigger(%this, %obj, %slot, %state)
	{
		%client = %obj.getControllingClient();
		%cam = %client.getControlObject();

		if(%cam.getClassName() $= "Player")
			return;
			
		%orbit = %cam.getOrbitObject();
		if (isObject(%pl = %client.player))
		{
			if (%pl.currResting && %state)
				%pl.WakeUp();

			if (%pl.unconscious)
				return;

			if (%pl.isSlipping)
				return;
		}
		else if (%slot == 0 && %state && !isObject(%orbit))
		{
			%range = 48;

			%a = %cam.getEyePoint();
			%b = vectorAdd(%a, vectorScale(%cam.getEyeVector(), %range));

			%mask =
				$TypeMasks::FxBrickObjectType |
				$TypeMasks::PlayerObjectType |
				$TypeMasks::CorpseObjectType |
				$TypeMasks::StaticShapeObjectType |
				$TypeMasks::TerrainObjectType |
				$TypeMasks::ItemObjectType;

			%ray = containerRayCast(%a, %b, %mask, %cam);
			if(isObject(%ray))
			{
				%client.examineObject(%ray);
			}
			initContainerRadiusSearch(getWords(%ray, 1, 3), 0.1,
				$TypeMasks::StaticShapeObjectType | $TypeMasks::CorpseObjectType);

			if (isObject(%col = containerSearchNext()))
			{
				%client.examineObject(%col);
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
	function gameConnection::applyBodyColors(%cl,%o) 
	{
		parent::applyBodyColors(%cl,%o);
		if(isObject(%pl = %cl.player))
		{
			if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/m_df.dts")
				%pl.applyAppearance(%cl.character);
		}
	}
	function gameConnection::applyBodyParts(%cl,%o) 
	{
		parent::applyBodyParts(%cl,%o);
		if(isObject(%pl = %cl.player))
		{
			if((%pl.getDatablock()).shapeFile $= "base/data/shapes/player/m_df.dts")
				%pl.applyAppearance(%cl.character);
		}
	}
	function Armor::onCollision(%this, %obj, %col, %velocity, %speed)
	{
		if (isObject(%col) && %col.getClassName() $= "Item" && %obj.client.miniGame == $defaultMiniGame)
		{
			if(!%col.static && %col.canPickup)
			{
				%slipcheck = (%obj.character.trait["Clumsy"] && vectorLen(%obj.getVelocity()) > 3 && getRandom() > 0.1) || (vectorLen(%obj.getVelocity()) > 0.1 && %col.getDatablock().slip);
				if(%slipcheck && !isEventPending(%obj.wakeUpSchedule) && $Sim::Time - %obj.lastSlip > 3)
				{
					%obj.slip(%col.getDatablock().slip);
					%obj.attackCount++;
					%obj.attackType[%obj.attackCount] = "slip";
					%obj.attackDot[%obj.attackCount] = 0; //lol who cares
					%obj.attackSource[%obj.attackCount] = %col.sourceObject;
					%obj.attackTime[%obj.attackCount] = $Sim::Time;
					%obj.attackDayTime[%obj.attackCount] = getDayCycleTime();
					%obj.attackDay[%obj.attackCount] = $days;
				}
				%col.setTransform(getWords(%col.getTransform(), 0, 2) SPC getWords(%obj.getTransform(), 3, 7));
				%col.setCollisionTimeout(%obj);
				%col.setVelocity(vectorScale(%obj.getVelocity(), 1.5));
			}
			return;
		}
		Parent::onCollision(%this, %obj, %col, %velocity, %speed);
	}
	function serverCmdDropTool(%client, %index)
	{
		if(!isObject(%player = %client.player))
			return parent::serverCmdDropTool(%client, %index);
		%player.dropTool(%index);
	}
	function serverCmdSit(%client)
	{
		if($DespairTrial)
			return;
		parent::serverCmdSit(%client);
	}
	function serverCmdUseTool(%client, %slot)
	{
		if(isObject(%pl = %client.player) && %pl.unconscious)
			return;
		parent::serverCmdUseTool(%client, %slot);
	}
	function serverCmdUnUseTool(%client, %slot)
	{
		if(isObject(%pl = %client.player) && %pl.unconscious)
			return;
		parent::serverCmdUnUseTool(%client, %slot);
	}
};
activatePackage(DespairPlayerPackage);