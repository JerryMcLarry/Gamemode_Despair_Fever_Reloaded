datablock ItemData(HatPartyhatItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Partyhat.dts";
	image = HatPartyhatImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Partyhat";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
};
datablock ShapeBaseImageData(HatPartyhatImage)
{
	item = HatPartyhatItem;
	shapeFile = "./Partyhat.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
