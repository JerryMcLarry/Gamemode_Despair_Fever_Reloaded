datablock AudioProfile(DoorJiggleSound) {
	fileName = $Despair::Path @ "res/sounds/door_jiggle.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(DoorKnockSound) {
	fileName = $Despair::Path @ "res/sounds/knock.wav";
	description = audioClose3D;
	preload = true;
};

datablock AudioProfile(DoorLockSound) {
	fileName = $Despair::Path @ "res/sounds/Lock.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(DoorUnlockSound) {
	fileName = $Despair::Path @ "res/sounds/Unlock.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(WoodHitSound)
{
	fileName = $Despair::Path @ "res/sounds/woodhit.wav";
	description = audioClose3D;
	preload = true;
};

function fxDtsBrick::doorDamage(%brick, %damage)
{
	if (!%brick.getDatablock().isDoor || %brick.doorMaxHits $= "")
		return;

	%brick.doorHits += %damage;
	if (%brick.doorHits >= %brick.doorMaxHits)
	{
		%brick.doorOpen(%brick.isCCW, %obj.client);
		%brick.lockState = false;
		%brick.broken = true;
		RS_Log("[DMGLOG] Door" SPC $roomNum[strReplace(%brick.lockID, "R", "")] SPC "was broken", "\c4");
	}
	else
		RS_Log("[DMGLOG] Door" SPC $roomNum[strReplace(%brick.lockID, "R", "")] SPC "was damaged", "\c4");
}

package DespairDoors
{
	function FxDTSBrick::door(%this, %state, %client)
	{
		if (%this.broken)
			%client.centerPrint("\c2The door lock is broken...", 2);
		else if (%this.lockId !$= "" && %this.lockState)
		{
			%client.centerPrint("\c2The door is locked.", 2);
			serverPlay3d(DoorJiggleSound, %this.getWorldBoxCenter(), 1);
			return;
		}
		Parent::door(%this, %state, %client);
	}
};

activatePackage("DespairDoors");