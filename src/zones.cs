datablock triggerData(SoundZoneTriggerData)
{
	tickPeriodMS = 500;
	parent = 0;
};

function SoundZoneTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	parent::onEnterTrigger(%this, %trigger, %obj);
	%obj.currentSoundZone = %trigger;
	talk("wow");
}

function SoundZoneTriggerData::onLeaveTrigger(%this, %trigger, %obj)
{
	%obj.currentSoundZone = "";
	talk("un-wow");
	parent::onLeaveTrigger(%this, %trigger, %obj);
}

function createSoundZone(%a, %b)
{
	%scale = vectorSub(%a, %b);
	%scale = mAbs(getWord(%scale, 0)) SPC mAbs(getWord(%scale, 1)) SPC mAbs(getWord(%scale, 2)); //absolute value
	%pos = vectorScale(vectorAdd(%a, %b), 0.5);
	%trigger = new Trigger()
	{
		datablock = SoundZoneTriggerData;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		position = %pos;
		rotation = "1 0 0 0";
		scale = %scale;
	};
	MissionCleanup.add(%trigger);
	%trigger.active = true;
	$line = drawLine($line, getWords(%trigger.getWorldBox(), 0, 2), getWords(%trigger.getWorldBox(), 3, 5), "0 1 1 1");
	return %trigger;
}