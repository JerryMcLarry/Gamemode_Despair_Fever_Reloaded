datablock AudioProfile(VoicePain1Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_pain-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain2Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_pain-02.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain3Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_pain-03.wav";
	description = audioClose3D;
	preload = true;
};
$painSoundCount["male"] = 3;

datablock AudioProfile(VoiceDeath1Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath2Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-02.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath3Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-03.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath4Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_Die-04.wav";
	description = audioClose3D;
	preload = true;
};
$deathSoundCount["male"] = 4;

datablock AudioProfile(VoiceShock1Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-01.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock2Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-02.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock3Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-03.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock4Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-04.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock5Male) {
	fileName = $Despair::Path @ "res/sounds/voice/male_shock-05.wav";
	description = AudioDefault3d;
	preload = true;
};
$shockSoundCount["male"] = 5;


datablock AudioProfile(VoicePain1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/female_pain-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/female_pain-02.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoicePain3Female) {
	fileName = $Despair::Path @ "res/sounds/voice/female_pain-03.wav";
	description = audioClose3D;
	preload = true;
};
$painSoundCount["female"] = 3;

datablock AudioProfile(VoiceDeath1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Pain-04.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Pain-05.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceDeath3Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Pain-06.wav";
	description = audioClose3D;
	preload = true;
};
$deathSoundCount["female"] = 3;

datablock AudioProfile(VoiceShock1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_shock-01.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_shock-02.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(VoiceShock3Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_shock-03.wav";
	description = AudioDefault3d;
	preload = true;
};
$shockSoundCount["female"] = 3;

datablock AudioProfile(VoiceCheese1Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Death-01.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(VoiceCheese2Female) {
	fileName = $Despair::Path @ "res/sounds/voice/Female_Death-02.wav";
	description = audioClose3D;
	preload = true;
};

datablock ParticleData(AlarmParticle)
{
	dragCoefficient      = 5.0;
	gravityCoefficient   = 0.0;
	inheritedVelFactor   = 0.0;
	windCoefficient      = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 0;
	useInvAlpha          = false;
	textureName          = "base/data/particles/exclamation";
	colors[0]     = "1 1 1 1";
	colors[1]     = "1 1 1 1";
	colors[2]     = "1 1 1 1";
	sizes[0]      = 0.9;
	sizes[1]      = 0.9;
	sizes[2]      = 0.9;
	times[0]      = 0.0;
	times[1]      = 0.2;
	times[2]      = 1.0;
};

datablock ParticleEmitterData(AlarmEmitter)
{
	ejectionPeriodMS = 35;
	periodVarianceMS = 0;
	ejectionVelocity = 0.0;
	ejectionOffset   = 1.8;
	velocityVariance = 0.0;
	thetaMin         = 0;
	thetaMax         = 0;
	phiReferenceVel  = 0;
	phiVariance      = 0;
	overrideAdvance = false;
	lifeTimeMS = 100;
	particles = "AlarmParticle";

	doFalloff = false; //if we do fall off with this emitter it ends up flickering, for most emitters you want this TRUE

	emitterNode = GenericEmitterNode;        //used when placed on a brick
	pointEmitterNode = TenthEmitterNode; //used when placed on a 1x1 brick

	uiName = "Emote - Alarm";
};

datablock ExplosionData(AlarmExplosion)
{
	lifeTimeMS = 2000;
	emitter[0] = AlarmEmitter;
	soundProfile = "";
};

//we cant spawn explosions, so this is a workaround for now
datablock ProjectileData(AlarmProjectile)
{
	explosion           = AlarmExplosion;

	armingDelay         = 0;
	lifetime            = 10;
	explodeOnDeath		= true;

	uiName = "Alarm Emote";
};

function Player::playShock(%player)
{
	if(isObject(%player.character))
		%gender = %player.character.gender;
	if(%gender $= "")
		%gender = "male";
	%player.playAudio(0, VoiceShock @ getRandom(1,$shockSoundCount[%gender]) @ %gender);
	if(isObject(%client = %player.client))
	RS_Log(%client.getPlayerName() SPC "(" @ getCharacterName(%client.character, 1) @ ") [" @ %client.getBLID() @ "] screamed.", "\c1");
}

//Function to check for "suspicious" things around them, 0 = calm, 1 = masked unknowns, armed people, 2 = blood, 3 = CORPSE!!!!!
function Player::getEnvironmentStress(%player)
{
	%stress = 0;
	%foundCorpse = 0;

	%center = %player.getEyePoint();
	initContainerRadiusSearch(%center, 64, $TypeMasks::PlayerObjectType | $TypeMasks::CorpseObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::ItemObjectType);
	while (isObject(%obj = containerSearchNext()))
	{
		if(%obj == %player)
			continue;
		%point = vectorAdd(%obj.getPosition(), "0 0 1");
		if(%obj.getType() & $TypeMasks::PlayerObjectType)
			%point = %obj.getEyePoint();
		%ray = containerRayCast(%center, %point, $TypeMasks::FxBrickObjectType, %player);

		%hasweapon = isObject(%img = %obj.getMountedImage(0)) && %img.item.className $= "DespairWeapon";
		%disguised = isObject(%img = %obj.getMountedImage(2)) && %img.item.disguise;

		if(!isObject(%ray) && %player.isWithinView(%point))
		{
			if(%stress < 1 && (%hasweapon || %disguised))
				%stress = 1; //Stress level 1, creepy people
			if(%stress < 2 && (%obj.isBlood || (isObject(%obj.itemProps) && %obj.itemProps.bloody) || %obj.bloody))
				%stress = 2; //Stress level 2, blood
			if(%stress < 3 && (%obj.isMurdered && %obj.getState() $= "Dead"))
			{
				%stress = 3; //OH SHIT BODY
				%foundCorpse = %obj;
			}
		}
	}

	return %stress SPC %foundCorpse;
}

function serverCmdAlarm(%client)
{
	if($DespairTrial)
		return DespairTrialOnAlarm(%client);

	if(isObject(%player = %client.player))
	{
		if(%player.unconscious)
			return;

		%ges = %player.getEnvironmentStress();
		%scream = getWord(%ges, 0); //doesn't matter which value we have, we can scream at anything suspicious
		%foundCorpse = getWord(%ges, 1);

		if(%client.killer)
			%scream = true; //killers can scream whenever the fuck they want

		if(%player.health <= 0)
			%scream = true; //crit people can scream whenever the fuck they want too!

		if (!%scream || $Sim::Time - %player.lastScream < %player.screamDelay)
			return;

		if ($Sim::Time - %player.lastScream < %player.screamDelay * 1.5)
			%player.screamDelay *= 1.5;
		else
			%player.screamDelay = 3;

		if(%player.health <= 0)
		{
			%player.screamDelay = 10;
			%player.health -= 5;
			messageClient(%client, '', "\c5You use some of your strength to scream!");
		}

		%player.screamDelay = getMin(%player.screamDelay, 10);

		%player.lastScream = $Sim::Time;
		%player.playShock();

		if(isObject(%foundCorpse))
		{
			%player.emote(AlarmProjectile);
			DespairCheckInvestigation(%player, %foundCorpse);
		}
	}
}

package DespairVoice
{
	function Player::playPain(%player)
	{
		if(isObject(%player.character))
		{
			%gender = %player.character.gender;
		}
		if(%gender $= "")
			%gender = "male";
		%player.stopAudio(0);
		%player.playAudio(0, VoicePain @ getRandom(1,$painSoundCount[%gender]) @ %gender);
	}
	function Player::playDeathCry(%player)
	{
		if(isObject(%player.character))
			%gender = %player.character.gender;
		if(%gender $= "")
			%gender = "male";
		%player.stopAudio(0);
		%player.playAudio(0, VoiceDeath @ getRandom(1,$deathSoundCount[%gender]) @ %gender);
	}
};
activatePackage("DespairVoice");