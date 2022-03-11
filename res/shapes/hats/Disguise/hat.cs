datablock ItemData(HatDisguiseItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Disguise.dts";
	image = HatDisguiseImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Disguise";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	disguiseName = "Villain";
	hidehair = false;
};
datablock ShapeBaseImageData(HatDisguiseImage)
{
	item = HatDisguiseItem;
	shapeFile = "./Disguise.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
