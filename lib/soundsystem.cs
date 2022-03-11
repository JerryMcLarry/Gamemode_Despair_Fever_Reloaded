function GameConnection::playGameSound(%this, %profile, %position)
{
	if (%position $= "") //No position provided? Skip the pleasantries and play it as 2d no matter what.
	{
		%this.play2d(%profile);
		return;
	}
	%description = %profile.description;
	if (isObject(%this.player)) //Check for soundproof zones and other things
	{
		%foundZone = getZoneFromPos(%position);
		%playerZone = %this.player.currentSoundZone;
		if(isObject(%playerZone) && %playerZone.active && %playerZone != %foundZone)
			return;
		if (%this.player.unconscious && !%description.is2d)
		{
			%dist = vectorDist(%this.player.getEyePoint(), %position);
			//if (%dist > 24) //Unconscious players can only hear sounds right next to them
			//	return;
			//Below we adjust the position in a proper way so it's heard relative to camera. This is done due to unconsciousness setting player's camera somewhere else.
			//First, we make sound pos relative to player (vectorsub). Second, we make it relative to camera position (vectoradd).
			%position = vectorAdd(vectorSub(%position, %this.player.getEyePoint()), getWords(%this.camera.getTransform(), 0, 2));
			//The next problem is the rotation issue - play3d doesn't take transform, so we have to rotate the %position ourselves.
			%rotation = axisToEuler(getWords(%this.player.getTransform(), 3, 6));
			%zrot = getWord(%rotation, 2);
			%camrot = getWord(axisToEuler(getWords(%this.camera.getTransform(), 3, 6)), 2); //Get Z rotation of camera
			%position = RotatePointAroundPivot(%position, getWords(%this.camera.getTransform(), 0, 2), mClampF(%zrot-%camrot, -180, 180));
		}
	}
	if (%description.is2d)
		%this.play2d(%profile);
	else
		%this.play3d(%profile, %position);
}

function getZoneFromPos(%position)
{
	initContainerRadiusSearch(%position, 0.25, $typeMasks::triggerObjectType);

	while(isObject(%trigger = containerSearchNext()))
	{
		if(%trigger.getDatablock() == SoundZoneTriggerData.getID())
		{
			return %trigger;
		}
	}
	return 0;
}

package DespairSoundSystem
{
	function serverPlay3d(%profile, %position) //Replace serverPlay3d
	{
		%count = ClientGroup.getCount();

		for (%i = 0; %i < %count; %i++)
			ClientGroup.getObject(%i).playGameSound(%profile, %position);
	}
};
activatePackage("DespairSoundSystem");

//Thanks to Wrapperup for this one, though iirc it only works around z rotation
function RotatePointAroundPivot(%point, %pivot, %zrot)
{
	%dist = vectorDist(%point, %pivot);
	
	%norm = vectorNormalize(vectorSub(%point, %pivot));
	
	%xB = getWord(%norm, 0);
	%yB = getWord(%norm, 1);
	
	%angle = mRadToDeg(mATan(%xB,%yB));
	
	%newAngle = %angle + %zrot;
	
	%pos = mSin(mDegToRad(%newAngle)) SPC mCos(mDegToRad(%newAngle)) SPC 0;
	
	%pos = vectorScale(%pos, %dist);
	%pos = vectorAdd(%pos, %pivot);
	
	return %pos;
}