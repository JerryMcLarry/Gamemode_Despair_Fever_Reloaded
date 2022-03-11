//Contains some 3d sound effects, rest of the sounds can be found in their respective script files
//Some descriptions
datablock AudioDescription(AudioQuiet3d : AudioClose3d)
{
	maxDistance = 15;
	referenceDistance = 3;
};

datablock AudioDescription(AudioQuietLooping3d : AudioClose3d)
{
	maxDistance = 15;
	referenceDistance = 3;
	isLooping = 1;
};

datablock AudioProfile(DescerationSound1)
{
	fileName = $Despair::Path @ "res/sounds/gore/desceration-01.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(DescerationSound2)
{
	fileName = $Despair::Path @ "res/sounds/gore/desceration-02.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(DescerationSound3)
{
	fileName = $Despair::Path @ "res/sounds/gore/desceration-03.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(DescerationSound4)
{
	fileName = $Despair::Path @ "res/sounds/gore/desceration-04.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BoneSound1)
{
	fileName = $Despair::Path @ "res/sounds/gore/bone_break-01.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BoneSound2)
{
	fileName = $Despair::Path @ "res/sounds/gore/bone_break-02.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BoneSound3)
{
	fileName = $Despair::Path @ "res/sounds/gore/bone_break-03.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(EatSound)
{
	fileName = $Despair::Path @ "res/sounds/eat.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(ShoveSound)
{
	fileName = $Despair::Path @ "res/sounds/gore/shove.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(ClickSound1)
{
	fileName = $Despair::Path @ "res/sounds/click1.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock AudioProfile(ClickSound2)
{
	fileName = $Despair::Path @ "res/sounds/click2.wav";
	description = AudioQuiet3d;
	preload = true;
};

//Motel sounds
datablock AudioProfile(TrashOpenSound)
{
	fileName = $Despair::Path @ "res/sounds/environment/TrashOpen.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(TrashCloseSound)
{
	fileName = $Despair::Path @ "res/sounds/environment/TrashClose.wav";
	description = audioClosest3D;
	preload = true;
};


datablock AudioProfile(CarHonkSound1)
{
	fileName = $Despair::Path @ "res/sounds/environment/CarHonk1.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(CarHonkSound2)
{
	fileName = $Despair::Path @ "res/sounds/environment/CarHonk2.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(CarHonkSound3)
{
	fileName = $Despair::Path @ "res/sounds/environment/CarHonk3.wav";
	description = AudioDefault3d;
	preload = true;
};

datablock AudioProfile(TruckHonkSound1)
{
	fileName = $Despair::Path @ "res/sounds/environment/TruckHonk1.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(TruckHonkSound2)
{
	fileName = $Despair::Path @ "res/sounds/environment/TruckHonk2.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(TruckHonkSound3)
{
	fileName = $Despair::Path @ "res/sounds/environment/TruckHonk3.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(BigTruckHonkSound)
{
	fileName = $Despair::Path @ "res/sounds/environment/rllybigtruckhonk.wav";
	description = AudioDefault3d;
	preload = true;
};