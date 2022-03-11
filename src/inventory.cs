function GameConnection::startViewingInventory(%this, %target, %maxtools)
{
	%player = %this.player;

	if (!isObject(%player))
		return;

	if (!isObject(%target))
		return;

	%player.unMountImage(0);
	%player.isViewingInventory = 1;
	%player.inventoryTarget = %target;
	%player.invMaxTools = %maxTools;
	%this.checkDistanceLoop(%target);
	%this.updateInventoryView();
}

function GameConnection::stopViewingInventory(%this)
{
	%player = %this.player;

	if (!isObject(%player))
		return;

	%player.isViewingInventory = 0;
	cancel(%this.inventoryDistanceSchedule);
	%this.updateInventoryView();
}

function GameConnection::updateInventoryView(%this)
{
	%player = %this.player;

	if (!isObject(%player) || !%player.isViewingInventory || !isObject(%target = %player.inventoryTarget))
	{
		if(isObject(%player))
		{
			//commandToClient(%this,'PlayGui_CreateToolHud',%player.getDataBlock().maxTools);
			for(%i=0; %i<%player.invMaxTools; %i++)
			{
				messageClient(%this, 'MsgItemPickup', '', %i, %player.tool[%i], 1);
			}
		}
		return;
	}

	if (vectorDist(%player.getPosition(), getWords(%target.getTransform(), 0, 2)) > 6)
	{
		%this.stopViewingInventory();
		return;
	}

	//commandToClient(%this,'PlayGui_CreateToolHud',%player.invMaxTools);
	for(%i=0; %i<%player.invMaxTools; %i++)
	{
		messageClient(%this, 'MsgItemPickup', '', %i, %target.tool[%i], %i != 0);
	}
	commandToClient(%this, 'SetActiveTool', %player.currTool);
}

function GameConnection::checkDistanceLoop(%this, %target)
{
	cancel(%this.checkDistanceLoop);
	if (!isObject(%player = %this.player) || !isObject(%target))
	{
		%this.stopViewingInventory();
		return;
	}
	if (vectorDist(%player.getPosition(), %target.getPosition()) > 6)
	{
		%this.stopViewingInventory();
		return;
	}
	%this.checkDistanceLoop = %this.schedule(500, checkDistanceLoop, %target);
}

package DespairInventory
{
	function serverCmdUseTool(%client, %slot)
	{
		if(isObject(%client.player) && %client.player.isViewingInventory)
		{
			%client.player.currTool = %slot;
			return;
		}
		Parent::serverCmdUseTool(%client, %slot);
	}
	function serverCmdUnUseTool(%client, %slot)
	{
		if(isObject(%client.player) && %client.player.isViewingInventory)
		{
			%client.stopViewingInventory();
		}
		Parent::serverCmdUnUseTool(%client, %slot);
	}
	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		if(%obj.isViewingInventory)
		{
			if(%slot == 0 && %state)
			{
				%target = %obj.inventoryTarget;
				%item = %target.tool[%obj.currTool];
				if(!isObject(%item))
					return;
				%itemName = %item.getName();
				%props = %target.itemProps[%obj.currTool];
				%slot = -1;
				if(%item.className $= "DespairWeapon")
				{
					if(%target.getMountedImage(0) == %item.image.getID())
					{
						if(%target.isDead)
						{
							if(isObject(%obj.client))
								messageClient(%obj.client, '', "\c3Rigor Mortis prevents you from pulling the weapon from the corpse's hands...");
							return;
						}
						else
						{
							%target.unMountImage(0);
						}
					}
					if(%obj.tool[%obj.weaponSlot] == nameToID(noWeaponIcon))
						%slot = %obj.setTool(%obj.weaponSlot, %item, %props, 1, 2);
				}
				else if(%item.className $= "Hat")
				{
					if(%item.onPickup("", %obj))
						%slot = %obj.hatSlot;
				}
				else if(!%item.isIcon && %obj.addTool(%item, %props, 1, 2) != -1)
				{
					%target.removeTool(%obj.currTool, 1, 2);
					%target.itemProps[%obj.currTool] = "";
					%slot = %obj.currTool;
				}
				if(isFunction(%itemName, "onDrop"))
					%item.onDrop(%target, %slot);
				if(%slot !$= "" && %slot != -1)
				{
					%obj.playThread(2, "activate2");
					%target.playThread(2, "plant");
					ServerPlay3D("BodyPickUpSound" @ getRandom(1, 3), %target.getPosition());
					if(isObject(%target.client))
						messageClient(%target.client, 'MsgItemPickup', '', %slot, %target.tool[%slot], 0);
					if(isObject(%obj.client))
					{
						messageClient(%obj.client, 'MsgItemPickup', '', %slot, %target.tool[%slot], 0);
						RS_Log(%obj.client.getPlayerName() SPC "(" @ %obj.client.getBLID() @ ") looted " @ %target.client.getPlayerName() SPC "(" @ %target.client.getBLID() @ ") of his " @ %target.tool[%slot].uiname, "\c1");
					}
				}
				return;
			}
		}
		Parent::onTrigger(%this, %obj, %slot, %state);
	}
};

activatePackage("DespairInventory");