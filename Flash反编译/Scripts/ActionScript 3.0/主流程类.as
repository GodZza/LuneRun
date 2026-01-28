
// ============================================================================
// LuneRun - Flash Game Core Classes
// 主流程类 - 游戏核心玩法代码
// ============================================================================

// ============================================================================
// Player - 玩家控制类
// ============================================================================
package com.playchilla.runner.player 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import com.playchilla.runner.track.entity.*;
    import flash.ui.*;
    import shared.input.*;
    import shared.math.*;
    import shared.sound.*;
    
    public class Player extends Object
    {
        public function Player(arg1:com.playchilla.runner.Level, arg2:shared.input.KeyboardInput, arg3:shared.input.MouseInput, arg4:shared.math.Vec3Const)
        {
            super();
            this._level = arg1;
            this._track = arg1.getTrack();
            this._keyboard = arg2;
            this._mouse = arg3;
            this._pos.copy(arg4);
            this._pos.y = 151;
            this._startPos.copy(arg4);
            this._world = this._level.getWorld();
            com.playchilla.runner.Audio.Sound.getSound(SBreath).loop(0);
            return;
        }

        public function isOnGround():Boolean
        {
            return this._onGround;
        }

        public function getSpeedAlpha():Number
        {
            return this._vel.length() / _MaxSpeed;
        }

        public function getSpeedY():Number
        {
            return this._vel.y;
        }

        public function getCurrentPart():com.playchilla.runner.track.Part
        {
            return this._currentPart;
        }

        public function setListener(arg1:com.playchilla.runner.player.IPlayerListener):void
        {
            this._listener = arg1;
            return;
        }

        public function hasCompleted():Boolean
        {
            return this._hasCompleted;
        }

        public function isDead():Boolean
        {
            return this._dead;
        }

        public function getSpeed():Number
        {
            return this._speed;
        }

        public function getForwardDir():shared.math.Vec3Const
        {
            return this._vel;
        }

        public function tick(arg1:int):void
        {
            if (this._currentPart == null) 
            {
                this._currentPart = this._track.getClosestPart(this._pos);
            }
            this._setWantedSpeeds();
            this._clip();
            this._entityInteraction();
            if (this._pos.y < 1) 
            {
                this._dead = true;
            }
            if (this._onGround) 
            {
                if (this._breathOn && Math.random() > 0.99) 
                {
                    this._breathOn = false;
                    com.playchilla.runner.Audio.Sound.getSound(SBreath).setVolume(0, 500);
                }
                else if (!this._breathOn && Math.random() > 0.99) 
                {
                    this._breathOn = true;
                    com.playchilla.runner.Audio.Sound.getSound(SBreath).setVolume(this.getSpeedAlpha(), 500);
                }
            }
            return;
        }

        internal function _entityInteraction():void
        {
            var loc1:*=this._world.getClosestEntity(this._pos, 1);
            if (loc1 is com.playchilla.runner.track.entity.SpeedEntity) 
            {
                this._vel.scaleSelf(1.2);
                loc1.remove();
            }
            return;
        }

        internal function _setWantedSpeeds():void
        {
            var loc2:*=false;
            var loc1:*=this._keyboard.isDown(flash.ui.Keyboard.SPACE) || this._mouse.isDown();
            if (this._onGround) 
            {
                loc2 = this._keyboard.isReleased(flash.ui.Keyboard.SPACE) || this._mouse.hasRelease();
                this._setWantedVelOnGround(loc1, loc2);
            }
            else 
            {
                this._setWantedVelInAir(loc1);
            }
            return;
        }

        internal function _setWantedVelOnGround(arg1:Boolean, arg2:Boolean):void
        {
            var loc1:*=NaN;
            this._speed = this._vel.dot(this._currentPart.dir);
            this._vel.copy(this._currentPart.dir.scale(this._speed));
            if (arg1) 
            {
                this._fallTime = 0;
                if (this._speed < _MaxSpeed) 
                {
                    this._vel.addSelf(this._currentPart.dir.scale(0.1));
                }
                loc1 = -0.1 * this._currentPart.dir.y;
                if (this._speed < _MaxSpeed * 1.4) 
                {
                    if (loc1 > 0) 
                    {
                        this._vel.addSelf(this._currentPart.dir.scale(loc1));
                    }
                    else 
                    {
                        this._vel.addSelf(this._currentPart.dir.scale(loc1));
                    }
                }
            }
            else if (arg2) 
            {
                this._vel.y = this._vel.y + Math.min(4, 1 + this._speed);
                this._onJump();
            }
            return;
        }

        internal function _setWantedVelInAir(arg1:Boolean):void
        {
            if (arg1) 
            {
                this._vel.y = Math.min(0, this._vel.y);
                this._vel.y = this._vel.y - 2 * _g;
            }
            var loc1:*;
            var loc2:*=((loc1 = this)._fallTime + 1);
            loc1._fallTime = loc2;
            this._vel.y = this._vel.y - _g;
            return;
        }

        internal function _clip():void
        {
            var loc1:*=null;
            var loc3:*=null;
            var loc4:*=null;
            var loc5:*=null;
            var loc6:*=false;
            var loc7:*=null;
            var loc8:*=null;
            var loc9:*=NaN;
            loc1 = this._pos.add(this._vel);
            com.playchilla.runner.Engine.pt.start("getClosestPart");
            var loc2:*=this._track.getClosestPart(loc1);
            com.playchilla.runner.Engine.pt.stop("getClosestPart");
            if (loc2 != null) 
            {
                loc3 = loc1.add(loc2.normal.scale(14));
                loc4 = loc1.sub(loc2.normal.scale(2));
                if (loc6 = !((loc5 = loc2.getSurface(loc3, loc4)) == null)) 
                {
                    this._currentPart = loc2;
                }
                if (!this._onGround && loc6) 
                {
                    this._onLand();
                }
                this._onGround = loc6;
            }
            if (this._onGround) 
            {
                this._pos.copy(loc5);
                loc7 = this._currentPart.getPos();
                loc9 = (loc8 = this._pos.sub(loc7)).dot(this._currentPart.right);
                if (Math.abs(loc9) > 2) 
                {
                    this._pos.addSelf(this._currentPart.right.rescale(loc9 > 0 ? -0.4 : 0.4));
                }
            }
            else 
            {
                this._pos.addSelf(this._vel);
            }
            return;
        }

        internal function _onJump():void
        {
            com.playchilla.runner.Audio.Sound.getSound(SBreath).setVolumeMod(0.25, 200);
            return;
        }

        internal function _onLand():void
        {
            var loc1:*=this._vel.clone();
            if (loc1.lengthSqr() < shared.math.Vec2Const.EpsilonSqr) 
            {
                return;
            }
            loc1.normalizeSelf();
            var loc2:*=Math.abs(loc1.dot(this._currentPart.normal));
            if (this._listener != null) 
            {
                this._listener.onLand(loc2);
            }
            if (loc2 > 0.26) 
            {
                if (this._listener != null) 
                {
                    this._listener.onLand(loc2);
                }
            }
            return;
        }

        public function destroy():void
        {
            com.playchilla.runner.Audio.Sound.getSound(SBreath).stop(0);
            return;
        }

        public function getPos():shared.math.Vec3Const
        {
            return this._pos;
        }

        internal const _pos:shared.math.Vec3=new shared.math.Vec3();

        internal const _startPos:shared.math.Vec3=new shared.math.Vec3();

        internal const _vel:shared.math.Vec3=new shared.math.Vec3();

        internal static const _MaxSpeed:Number=3.8;

        internal static const _g:Number=0.14;

        internal var _track:com.playchilla.runner.track.Track;

        internal var _keyboard:shared.input.KeyboardInput;

        internal var _currentPart:com.playchilla.runner.track.Part;

        internal var _fallTime:int=0;

        internal var _onGround:Boolean=false;

        internal var _listener:com.playchilla.runner.player.IPlayerListener=null;

        internal var _mouse:shared.input.MouseInput;

        internal var _hasCompleted:Boolean=false;

        internal var _dead:Boolean=false;

        internal var _level:com.playchilla.runner.Level;

        internal var _speed:Number=0;

        internal var _world:com.playchilla.runner.track.entity.World;

        internal var _breath:shared.sound.ISoundChannel;

        internal var _breathOn:Boolean=false;
    }
}


// ============================================================================
// Track - 轨道管理类
// ============================================================================
package com.playchilla.runner.track 
{
    import __AS3__.vec.*;
    import com.playchilla.runner.track.segment.*;
    import shared.algorithm.spatial.*;
    import shared.debug.*;
    import shared.math.*;
    
    public class Track extends Object
    {
        public function Track()
        {
            super();
            return;
        }

        public function getSegments():__AS3__.vec.Vector.<com.playchilla.runner.track.segment.Segment>
        {
            return this._segments;
        }

        public function getStartPart():com.playchilla.runner.track.Part
        {
            return this._segments[0].getFirstPart();
        }

        public function getTotalParts():int
        {
            return this._totalParts;
        }

        public function getFirstSegment():com.playchilla.runner.track.segment.Segment
        {
            return this._segments[0];
        }

        public function getLastSegment():com.playchilla.runner.track.segment.Segment
        {
            return this._segments[(this._segments.length - 1)];
        }

        public function getStartPos():shared.math.Vec3Const
        {
            return this._segments[0].getConnectPart().getPos();
        }

        public function getConnectPart():com.playchilla.runner.track.Part
        {
            return this.getLastSegment().getLastPart();
        }

        public function addSegment(arg1:com.playchilla.runner.track.segment.Segment):com.playchilla.runner.track.segment.Segment
        {
            var loc1:*=null;
            var loc2:*=null;
            if (this._segments.length > 0) 
            {
                loc2 = this._segments[(this._segments.length - 1)];
                loc2.setNextSegment(arg1);
            }
            this._segments.push(arg1);
            var loc3:*=0;
            var loc4:*=arg1.getParts();
            for each (loc1 in loc4) 
            {
                this._partGrid.add(loc1);
            }
            this._totalParts = this._totalParts + arg1.getParts().length;
            return arg1;
        }

        public function removeSegment(arg1:com.playchilla.runner.track.segment.Segment):void
        {
            var loc3:*=null;
            var loc4:*=null;
            var loc1:*=this._segments.indexOf(arg1);
            shared.debug.Debug.assert(!(loc1 == -1), "Trying to remove a segment that doesn\'t exist.");
            var loc2:*=0;
            while (loc2 <= loc1) 
            {
                loc3 = this._segments[loc2];
                var loc5:*=0;
                var loc6:*=loc3.getParts();
                for each (loc4 in loc6) 
                {
                    this._partGrid.remove(loc4);
                }
                loc3.remove();
                this._totalParts = this._totalParts - loc3.getParts().length;
                shared.debug.Debug.assert(this._totalParts >= 0, "Negative number of total parts after segment removal.");
                ++loc2;
            }
            this._segments.splice(0, loc1 + 1);
            return;
        }

        public function getClosestPart(arg1:shared.math.Vec3Const):com.playchilla.runner.track.Part
        {
            var loc1:*=NaN;
            var loc2:*=NaN;
            var loc3:*=NaN;
            var loc7:*=null;
            var loc8:*=null;
            var loc9:*=NaN;
            loc1 = arg1.x;
            loc2 = arg1.z;
            loc3 = com.playchilla.runner.track.Part.Length * 4;
            var loc4:*=this._partGrid.getOverlappingXY(loc1 - loc3, loc2 - loc3, loc1 + loc3, loc2 + loc3);
            var loc5:*=Number.MAX_VALUE;
            var loc6:*=null;
            var loc10:*=0;
            var loc11:*=loc4;
            for each (loc7 in loc11) 
            {
                if (!((loc9 = (loc8 = loc7.getPos()).distanceSqr(arg1)) < loc5)) 
                {
                    continue;
                }
                loc5 = loc9;
                loc6 = loc7;
            }
            return loc6;
        }

        public function isIntersectingXZ(arg1:int, arg2:int, arg3:Number):Boolean
        {
            return this._partGrid.getOverlappingXY(arg1 - arg3, arg2 - arg3, arg1 + arg3, arg2 + arg3).length > 0;
        }

        public function getRandomClosePos(arg1:com.playchilla.runner.track.Part, arg2:int):shared.math.Vec3Const
        {
            var loc3:*=null;
            var loc4:*=null;
            var loc5:*=null;
            var loc6:*=null;
            var loc7:*=0;
            var loc1:*=arg1.segment;
            arg2 = arg2 + arg1.partIndex;
            var loc2:*=0;
            while (!(loc1 == null) && loc2 < 40) 
            {
                loc3 = loc1.getParts();
                if (arg2 < loc3.length) 
                {
                    loc4 = loc3[arg2].getPos();
                    loc6 = (loc5 = loc3[arg2].dir).crossXYZ(0, 1, 0);
                    if ((loc7 = Math.random() * 200 - 100) > 0) 
                    {
                        loc7 = loc7 + 25;
                    }
                    else 
                    {
                        loc7 = loc7 - 25;
                    }
                    loc6.scaleSelf(loc7);
                    loc6.addSelf(loc4);
                    if (this._partGrid.getOverlappingXY(loc6.x - 10, loc6.z - 10, loc6.x + 10, loc6.z + 10).length == 0) 
                    {
                        return loc6;
                    }
                    return null;
                }
                arg2 = arg2 - loc3.length;
                loc1 = loc1.getNextSegment();
                ++loc2;
            }
            return null;
        }

        internal const _segments:__AS3__.vec.Vector.<com.playchilla.runner.track.segment.Segment>=new Vector.<com.playchilla.runner.track.segment.Segment>();

        internal const _partGrid:shared.algorithm.spatial.SpatialHash=new shared.algorithm.spatial.SpatialHash(com.playchilla.runner.track.Part.Length * 4, 10000001);

        internal var _totalParts:int=0;
    }
}


// ============================================================================
// TrackGenerator - 轨道生成器类
// ============================================================================
package com.playchilla.runner.track 
{
    import __AS3__.vec.*;
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.generator.*;
    import shared.math.*;
    
    public class TrackGenerator extends Object
    {
        public function TrackGenerator(arg1:com.playchilla.runner.Materials)
        {
            super();
            this._materials = arg1;
            return;
        }

        public function addSegmentGenerator(arg1:com.playchilla.runner.track.generator.ISegmentGenerator):void
        {
            this._generators.push(arg1);
            return;
        }

        public function generate(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:Number, arg4:int, arg5:int):int
        {
            var loc2:*=null;
            var loc1:*=0;
            for (;;) 
            {
                loc2 = this._generators[this._genIndex];
                this._genIndex = arg2.nextDouble() * this._generators.length;
                while (!loc2.canRun(this._lastGenerator, arg3, arg5)) 
                {
                    loc2 = this._generators[this._genIndex];
                    this._genIndex = arg2.nextDouble() * this._generators.length;
                }
                loc2.generate(this._lastGenerator, arg3, arg5);
                this._lastGenerator = loc2;
                if (!(++loc1 >= arg4)) 
                {
                    continue;
                }
                return loc1;
            }
            return loc1;
        }

        public function getLastGenerator():com.playchilla.runner.track.generator.ISegmentGenerator
        {
            return this._lastGenerator;
        }

        internal const _generators:__AS3__.vec.Vector.<com.playchilla.runner.track.generator.ISegmentGenerator>=new Vector.<com.playchilla.runner.track.generator.ISegmentGenerator>();

        internal var _materials:com.playchilla.runner.Materials;

        internal var _lastGenerator:com.playchilla.runner.track.generator.ISegmentGenerator=null;

        internal var _genIndex:int=0;
    }
}


// ============================================================================
// DynamicTrack - 动态轨道类
// ============================================================================
package com.playchilla.runner.track 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.generator.*;
    import com.playchilla.runner.track.segment.*;
    import shared.math.*;
    
    public class DynamicTrack extends Object
    {
        public function DynamicTrack(arg1:com.playchilla.runner.Level, arg2:int, arg3:int)
        {
            super();
            this._level = arg1;
            this._loadForward = arg2;
            this._keepBackward = arg3;
            var loc1:*=this._level.getMaterials();
            this._forwardGen = new com.playchilla.runner.track.generator.ForwardGenerator(this._track, this._rnd, loc1, this._level);
            this._longJumpGen = new com.playchilla.runner.track.generator.LongJumpGenerator(this._track, this._rnd, loc1);
            this._trackGenerator = new com.playchilla.runner.track.TrackGenerator(loc1);
            this._track.addSegment(new com.playchilla.runner.track.segment.ForwardSegment("ForwardSegment", 150, null, new shared.math.Vec3(0, 0, 1), 0, 0, 20, loc1, -1));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.ForwardGenerator(this._track, this._rnd, loc1, this._level));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.HoleGenerator(this._track, this._rnd, loc1));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.SlopeGenerator(this._track, this._rnd, loc1, true));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.SlopeGenerator(this._track, this._rnd, loc1, false));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.CurveGenerator(this._track, this._rnd, loc1));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.HillGenerator(this._track, this._rnd, loc1));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.LoopGenerator(this._track, this._rnd, loc1));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.LongJumpGenerator(this._track, this._rnd, loc1));
            this._trackGenerator.addSegmentGenerator(new com.playchilla.runner.track.generator.IslandGenerator(this._track, this._rnd, loc1));
            this._tutorialGenerator = new com.playchilla.runner.track.generator.TutorialGenerator(this._track, this._rnd, loc1);
            this._longJumpTutorial = new com.playchilla.runner.track.generator.LongJumpTutorialGenerator(this._track, this._rnd, loc1);
            return;
        }

        public function update(arg1:com.playchilla.runner.track.Part, arg2:int, arg3:int):Boolean
        {
            this._tryRemove(arg1);
            this._tryAdd(arg2, arg3);
            return this._generatedForLevel >= arg3;
        }

        internal function _tryRemove(arg1:com.playchilla.runner.track.Part):void
        {
            if (arg1 == null) 
            {
                return;
            }
            var loc1:*=arg1.segment.getPreviousSegment();
            if (loc1 == null) 
            {
                return;
            }
            var loc2:*=this._keepBackward;
            while (!(loc1 == null) && --loc2 > 0) 
            {
                loc1 = loc1.getPreviousSegment();
            }
            if (loc1 != null) 
            {
                this._track.removeSegment(loc1);
            }
            return;
        }

        internal function _tryAdd(arg1:int, arg2:int):void
        {
            var loc2:*=0;
            if (arg1 != this._lastGenerateLevelId) 
            {
                loc2 = arg1 + 1;
                if (arg1 != 26) 
                {
                    if (arg1 != 28) 
                    {
                        if (arg1 == 32) 
                        {
                            loc2 = 105;
                        }
                    }
                    else 
                    {
                        loc2 = 100;
                    }
                }
                else 
                {
                    loc2 = 101;
                }
                this._rnd.setSeed(loc2);
                this._lastGenerateLevelId = arg1;
                this._generatedForLevel = 0;
            }
            if (this._generatedForLevel >= arg2) 
            {
                return;
            }
            if (this._track.getSegments().length >= this._keepBackward + this._loadForward) 
            {
                return;
            }
            var loc1:*=1 - Math.max(0, Math.min((32 - arg1) / 32, 1));
            shared.debug.Debug.assert(loc1 >= 0 && loc1 <= 1, "Difficulty out of range: " + loc1);
            if (arg1 != 1) 
            {
                if (arg1 != 20) 
                {
                    if (arg1 == 32 && this._generatedForLevel == (arg2 - 1)) 
                    {
                        this._longJumpGen.setHoleParts(100);
                        this._longJumpGen.generate(this._trackGenerator.getLastGenerator(), 0.1, arg1);
                    }
                    else if (arg1 != 33) 
                    {
                        if (this._generatedForLevel != 0) 
                        {
                            this._trackGenerator.generate(this._track, this._rnd, loc1, 1, arg1);
                        }
                        else if (arg1 == 17) 
                        {
                            this._forwardGen.setParts(300);
                            this._forwardGen.generate(this._trackGenerator.getLastGenerator(), loc1, arg1);
                        }
                    }
                    else 
                    {
                        this._forwardGen.setParts(40);
                        this._forwardGen.generate(this._trackGenerator.getLastGenerator(), 0.1, arg1);
                    }
                }
                else 
                {
                    this._longJumpTutorial.generate(null, 0, arg1);
                }
            }
            else 
            {
                this._tutorialGenerator.generate(null, 0, arg1);
            }
            var loc3:*;
            var loc4:*=((loc3 = this)._generatedForLevel + 1);
            loc3._generatedForLevel = loc4;
            return;
        }

        public function getTrack():com.playchilla.runner.track.Track
        {
            return this._track;
        }

        public function isLastLevelPart(arg1:com.playchilla.runner.track.Part):Boolean
        {
            return arg1 == this._lastLevelPart;
        }

        internal const _track:com.playchilla.runner.track.Track=new com.playchilla.runner.track.Track();

        internal const _rnd:shared.math.Random=new shared.math.Random(1);

        internal var _trackGenerator:com.playchilla.runner.track.TrackGenerator;

        internal var _lastGenerateLevelId:int=-1;

        internal var _loadForward:int;

        internal var _keepBackward:int;

        internal var _generatedForLevel:int=0;

        internal var _tutorialGenerator:com.playchilla.runner.track.generator.TutorialGenerator;

        internal var _longJumpTutorial:com.playchilla.runner.track.generator.LongJumpTutorialGenerator;

        internal var _lastLevelPart:com.playchilla.runner.track.Part;

        internal var _level:com.playchilla.runner.Level;

        internal var _forwardGen:com.playchilla.runner.track.generator.ForwardGenerator;

        internal var _longJumpGen:com.playchilla.runner.track.generator.LongJumpGenerator;
    }
}


// ============================================================================
// Part - 轨道部件类
// ============================================================================
package com.playchilla.runner.track 
{
    import away3d.entities.*;
    import away3d.tools.utils.*;
    import com.playchilla.runner.track.segment.*;
    import flash.geom.*;
    import shared.algorithm.spatial.*;
    import shared.math.*;
    
    public class Part extends shared.algorithm.spatial.SpatialHashValue
    {
        public function Part(arg1:com.playchilla.runner.track.segment.Segment, arg2:shared.math.Vec3Const, arg3:shared.math.Vec3Const, arg4:shared.math.Vec3Const, arg5:away3d.entities.Mesh, arg6:int, arg7:Number)
        {
            var loc1:*=com.playchilla.runner.track.Part.Length;
            super(arg2.x - loc1, arg2.z - loc1, arg2.x + loc1, arg2.z + loc1);
            this.segment = arg1;
            this._pos.copy(arg2);
            this.dir.copy(arg3);
            this.dir.normalizeSelf();
            this.normal.copy(arg4);
            this.normal.normalizeSelf();
            this.right.copy(arg4.cross(arg3));
            this.zRot = arg7;
            this.mesh = arg5;
            this.partIndex = arg6;
            return;
        }

        public function getPos():shared.math.Vec3Const
        {
            return this._pos;
        }

        public function hasPassedForward(arg1:shared.math.Vec3Const):Boolean
        {
            var loc1:*=this._pos.add(this.dir.scale(Length * 0.5));
            return arg1.sub(loc1).dot(this.dir) > 0;
        }

        public function hasPassedBackward(arg1:shared.math.Vec3Const):Boolean
        {
            var loc1:*=this._pos.sub(this.dir.scale(Length * 0.5));
            return arg1.sub(loc1).dot(this.dir) < 0;
        }

        public function getSurface(arg1:shared.math.Vec3Const, arg2:shared.math.Vec3Const):shared.math.Vec3
        {
            var loc2:*=NaN;
            if (this.mesh == null) 
            {
                return null;
            }
            var loc1:*=new away3d.tools.utils.Ray();
            loc2 = (-Length) * 0.6;
            var loc3:*=new flash.geom.Vector3D(-loc2, 0, -loc2);
            var loc4:*=new flash.geom.Vector3D(loc2, 0, -loc2);
            var loc5:*=new flash.geom.Vector3D(loc2, 0, loc2);
            var loc6:*=new flash.geom.Vector3D(-loc2, 0, loc2);
            loc3.copyFrom(this.mesh.transform.transformVector(loc3));
            loc4.copyFrom(this.mesh.transform.transformVector(loc4));
            loc5.copyFrom(this.mesh.transform.transformVector(loc5));
            loc6.copyFrom(this.mesh.transform.transformVector(loc6));
            var loc7:*;
            if ((loc7 = loc1.getRayToTriangleIntersection(arg1.n(), arg2.n(), loc3, loc4, loc5)) != null) 
            {
                return shared.math.Vec3Const.fromN(loc7);
            }
            if ((loc7 = loc1.getRayToTriangleIntersection(arg1.n(), arg2.n(), loc3, loc6, loc5)) != null) 
            {
                return shared.math.Vec3Const.fromN(loc7);
            }
            return null;
        }

        internal const _pos:shared.math.Vec3=new shared.math.Vec3();

        public const dir:shared.math.Vec3=new shared.math.Vec3();

        public const normal:shared.math.Vec3=new shared.math.Vec3();

        public const right:shared.math.Vec3=new shared.math.Vec3();

        public static const Length:Number=6;

        public var zRot:Number=0;

        public var next:com.playchilla.runner.track.Part=null;

        public var previous:com.playchilla.runner.track.Part=null;

        public var mesh:away3d.entities.Mesh;

        public var segment:com.playchilla.runner.track.segment.Segment;

        public var partIndex:int;
    }
}


// ============================================================================
// Segment - 轨道段基类
// ============================================================================
package com.playchilla.runner.track.segment 
{
    import __AS3__.vec.*;
    import com.playchilla.runner.track.*;
    import shared.debug.*;
    
    public class Segment extends Object
    {
        public function Segment(arg1:com.playchilla.runner.track.Part, arg2:String, arg3:int)
        {
            super();
            shared.debug.Debug.assert(!(arg1 == null), "Segment without connect part.");
            this._connectPart = arg1;
            this._name = arg2;
            this._levelId = arg3;
            return;
        }

        public function addPart(arg1:com.playchilla.runner.track.Part):void
        {
            var loc1:*=null;
            loc1 = this.getLastPart();
            if (loc1 == null) 
            {
                this._connectPart.next = arg1;
            }
            else 
            {
                loc1.next = arg1;
                arg1.previous = loc1;
            }
            this._parts.push(arg1);
            return;
        }

        public function getParts():__AS3__.vec.Vector.<com.playchilla.runner.track.Part>
        {
            return this._parts;
        }

        public function getLastPart():com.playchilla.runner.track.Part
        {
            return this._parts.length > 0 ? this._parts[(this._parts.length - 1)] : null;
        }

        public function getConnectPart():com.playchilla.runner.track.Part
        {
            return this._connectPart;
        }

        public function getNextSegment():com.playchilla.runner.track.segment.Segment
        {
            return this._nextSegment;
        }

        public function getPreviousSegment():com.playchilla.runner.track.segment.Segment
        {
            return this._connectPart != null ? this._connectPart.segment : null;
        }

        public function getFirstPart():com.playchilla.runner.track.Part
        {
            return this._parts.length > 0 ? this._parts[0] : null;
        }

        public function getNumberOfParts():int
        {
            return this._parts.length;
        }

        public function setNextSegment(arg1:com.playchilla.runner.track.segment.Segment):void
        {
            shared.debug.Debug.assert(!(arg1 == this), "Trying to set next segment to self.");
            this._nextSegment = arg1;
            return;
        }

        public function remove():void
        {
            this._isRemoved = true;
            this._nextSegment._connectPart = null;
            return;
        }

        public function isRemoved():Boolean
        {
            return this._isRemoved;
        }

        public function toString():String
        {
            return this._name;
        }

        public function getLevelId():int
        {
            return this._levelId;
        }

        internal const _connectPart:com.playchilla.runner.track.Part;

        internal const _parts:__AS3__.vec.Vector.<com.playchilla.runner.track.Part>=new Vector.<com.playchilla.runner.track.Part>();

        internal var _nextSegment:com.playchilla.runner.track.segment.Segment=null;

        internal var _name:String;

        internal var _levelId:int;

        internal var _isRemoved:Boolean=false;
    }
}


// ============================================================================
// ForwardSegment - 前进轨道段
// ============================================================================
package com.playchilla.runner.track.segment 
{
    import away3d.entities.*;
    import away3d.materials.*;
    import away3d.primitives.*;
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import flash.geom.*;
    import shared.math.*;
    
    public class ForwardSegment extends com.playchilla.runner.track.segment.Segment
    {
        public function ForwardSegment(arg1:String, arg2:Number, arg3:com.playchilla.runner.track.Part, arg4:shared.math.Vec3Const, arg5:Number, arg6:Number, arg7:int, arg8:com.playchilla.runner.Materials, arg9:int, arg10:Boolean=true, arg11:Boolean=true)
        {
            var loc6:*=null;
            var loc7:*=0;
            var loc8:*=NaN;
            var loc9:*=null;
            this._material = arg8.getMaterial("part");
            if (arg3 == null) 
            {
                arg3 = new com.playchilla.runner.track.Part(null, new shared.math.Vec3(0, arg2, 0), arg4, new shared.math.Vec3(0, 1, 0), new away3d.entities.Mesh(this._cube, this._material), 0, 0);
            }
            super(arg3, arg1, arg9);
            var loc1:*;
            (loc1 = arg3.getPos().n().clone()).y = arg2;
            var loc2:*=arg5 / arg7;
            var loc3:*=arg6 / arg7;
            var loc4:*=(-arg5) * 0.5;
            var loc5:*=0;
            while (loc5 < arg7) 
            {
                (loc6 = new away3d.entities.Mesh(this._cube, this._material)).lookAt(arg4.n());
                loc6.rotate(flash.geom.Vector3D.X_AXIS, (loc5 + 1) * loc3);
                loc6.rotate(flash.geom.Vector3D.Y_AXIS, (loc5 + 1) * loc2);
                loc7 = 5;
                loc8 = loc4;
                if (arg10 && loc5 < loc7) 
                {
                    loc8 = loc4 * loc5 / loc7;
                }
                if (arg11 && loc5 >= arg7 - loc7) 
                {
                    loc8 = loc4 * (1 - (loc5 - (arg7 - loc7)) / loc7);
                }
                loc6.rotate(flash.geom.Vector3D.Z_AXIS, loc8);
                (loc9 = loc6.forwardVector).scaleBy(com.playchilla.runner.track.Part.Length);
                loc1.incrementBy(loc9);
                addPart(new com.playchilla.runner.track.Part(this, shared.math.Vec3Const.fromN(loc1), shared.math.Vec3Const.fromN(loc9), shared.math.Vec3Const.fromN(loc6.upVector), loc6, getParts().length, loc8));
                ++loc5;
            }
            return;
        }

        internal const _cube:away3d.primitives.CubeGeometry=new away3d.primitives.CubeGeometry(com.playchilla.runner.track.Part.Length * 1.1, 0.2, com.playchilla.runner.track.Part.Length - 0.5);

        internal var _material:away3d.materials.MaterialBase;
    }
}


// ============================================================================
// HoleSegment - 空洞轨道段
// ============================================================================
package com.playchilla.runner.track.segment 
{
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class HoleSegment extends com.playchilla.runner.track.segment.Segment
    {
        public function HoleSegment(arg1:com.playchilla.runner.track.Part, arg2:shared.math.Vec3Const, arg3:int, arg4:int)
        {
            var loc1:*=null;
            super(arg1, "HoleSegment", arg4);
            this._parts = arg3;
            (loc1 = arg2.clone()).y = 0;
            loc1.normalizeSelf();
            var loc2:*=arg1.getPos().clone();
            var loc3:*;
            (loc3 = loc1.clone()).scaleSelf(com.playchilla.runner.track.Part.Length);
            loc2.y = 0;
            var loc4:*=0;
            while (loc4 < (arg3 - 1)) 
            {
                loc2.addSelf(loc3);
                addPart(new com.playchilla.runner.track.Part(this, loc2, loc1, new shared.math.Vec3(0, 1, 0), null, getParts().length, 0));
                ++loc4;
            }
            return;
        }

        public override function getNumberOfParts():int
        {
            return this._parts;
        }

        internal var _parts:int;
    }
}


// ============================================================================
// ISegmentGenerator - 轨道段生成器接口
// ============================================================================
package com.playchilla.runner.track.generator 
{
    public interface ISegmentGenerator
    {
        function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean;

        function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void;
    }
}


// ============================================================================
// SegmentGenerator - 轨道段生成器基类
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import com.playchilla.runner.track.segment.*;
    import shared.math.*;
    
    public class SegmentGenerator extends Object
    {
        public function SegmentGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super();
            this._track = arg1;
            this._rnd = arg2;
            this._materials = arg3;
            return;
        }

        public function addForwardSegment(arg1:Number, arg2:Number, arg3:Number, arg4:int, arg5:int, arg6:Boolean=true, arg7:Boolean=true):com.playchilla.runner.track.segment.Segment
        {
            var loc1:*=this._track.getConnectPart();
            return this._track.addSegment(new com.playchilla.runner.track.segment.ForwardSegment("XSegment", arg1, loc1, loc1.dir, arg2, arg3, arg4, this._materials, arg5, arg6, arg7));
        }

        public function addHoleSegment(arg1:int, arg2:int):com.playchilla.runner.track.segment.Segment
        {
            return this._track.addSegment(new com.playchilla.runner.track.segment.HoleSegment(this._track.getConnectPart(), this._track.getConnectPart().dir, arg1, arg2));
        }

        public function getNextY(arg1:Number):Number
        {
            var loc1:*=null;
            var loc3:*=null;
            var loc6:*=null;
            var loc7:*=NaN;
            loc1 = this._track.getLastSegment();
            var loc2:*=loc1.getLastPart();
            if (!(loc1 is com.playchilla.runner.track.segment.HoleSegment)) 
            {
                return loc2.getPos().y;
            }
            var loc4:*=(loc3 = this.getLastSolidPart(this._track.getLastSegment())).getPos().y;
            var loc5:*=loc2.getPos();
            loc6 = loc3.getPos();
            loc7 = loc1.getNumberOfParts();
            var loc8:*=loc6.y;
            var loc9:*=25;
            var loc10:*=40;
            var loc11:*=1 - loc7 / loc10;
            var loc12:*=loc8 + loc11 * loc9;
            loc12 = Math.min(195, loc12);
            var loc13:*=Math.min(loc7 / 70, 1);
            var loc14:*;
            var loc15:*;
            return loc15 = (loc14 = Math.max(50, loc12 - loc12 * 2 * loc13)) + this._rnd.nextDouble() * (loc12 - loc14);
        }

        public function getLastSolidPart(arg1:com.playchilla.runner.track.segment.Segment):com.playchilla.runner.track.Part
        {
            var loc1:*=arg1;
            while (loc1 is com.playchilla.runner.track.segment.HoleSegment) 
            {
                loc1 = arg1.getPreviousSegment();
            }
            if (loc1 == null) 
            {
                return null;
            }
            return loc1.getLastPart();
        }

        protected var _track:com.playchilla.runner.track.Track;

        protected var _rnd:shared.math.Random;

        protected var _materials:com.playchilla.runner.Materials;
    }
}


// ============================================================================
// ForwardGenerator - 前进生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class ForwardGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function ForwardGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials, arg4:com.playchilla.runner.Level)
        {
            this._level = arg4;
            super(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg3 > 3 && arg1 is com.playchilla.runner.track.generator.ForwardGenerator) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.8 - 0.6 * arg2) 
            {
                return false;
            }
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            var loc1:*=getNextY(arg2);
            var loc2:*=this._parts != -1 ? this._parts : 13 + _rnd.nextDouble() * 30;
            if (_rnd.nextDouble() < 0.4 * arg2 && arg3 > 16) 
            {
                loc2 = Math.max(4, loc2 - (20 + 5 * arg2));
            }
            addForwardSegment(loc1, 0, 0, loc2, arg3);
            return;
        }

        public function setParts(arg1:int):void
        {
            this._parts = arg1;
            return;
        }

        internal var _level:com.playchilla.runner.Level;

        internal var _parts:int=-1;
    }
}


// ============================================================================
// HillGenerator - 山坡生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class HillGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function HillGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            this._up = new com.playchilla.runner.track.generator.SlopeGenerator(arg1, arg2, arg3, true);
            this._down = new com.playchilla.runner.track.generator.SlopeGenerator(arg1, arg2, arg3, false);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg3 < 3) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.07 - 0.04 * arg2) 
            {
                return false;
            }
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            var loc3:*=NaN;
            var loc4:*=null;
            var loc1:*=_rnd.nextIntRange(1, 2);
            var loc2:*=0;
            while (loc2 < loc1) 
            {
                if ((loc3 = _track.getConnectPart().getPos().y) > 190) 
                {
                    loc4 = this._down;
                }
                else if (loc3 < 110) 
                {
                    loc4 = this._up;
                }
                else 
                {
                    loc4 = loc3 > 150 ? this._down : this._up;
                }
                addForwardSegment(getNextY(arg2), 0, -35, 10, arg3);
                addForwardSegment(getNextY(arg2), 0, 35, 15, arg3);
                addForwardSegment(getNextY(arg2), 0, 35, 10, arg3);
                addForwardSegment(getNextY(arg2), 0, -35, 15, arg3);
                arg1 = loc4;
                ++loc2;
            }
            return;
        }

        internal var _up:com.playchilla.runner.track.generator.SlopeGenerator;

        internal var _down:com.playchilla.runner.track.generator.SlopeGenerator;
    }
}


// ============================================================================
// CurveGenerator - 弯道生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class CurveGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function CurveGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg2 < 0.07) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.4 + 0.45 * arg2) 
            {
                return false;
            }
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            var loc1:*=NaN;
            loc1 = (2 * _rnd.nextDouble() - 1);
            var loc2:*=(0.2 + arg2) * loc1 * 180;
            var loc3:*=10 + 10 * Math.abs(loc1) + _rnd.nextDouble() * (15 - 10 * arg2);
            var loc4:*=getNextY(arg2);
            addForwardSegment(loc4, 0, 0, 2, arg3);
            addForwardSegment(loc4, loc2, 0, loc3, arg3);
            return;
        }
    }
}


// ============================================================================
// HoleGenerator - 空洞生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import __AS3__.vec.*;
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import com.playchilla.runner.track.segment.*;
    import shared.math.*;
    
    public class HoleGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function HoleGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg1 == null) 
            {
                return false;
            }
            if (arg1 is com.playchilla.runner.track.generator.HoleGenerator || _track.getLastSegment() is com.playchilla.runner.track.segment.HoleSegment) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.85 + 0.15 * arg2) 
            {
                return false;
            }
            return true;
        }

        public function setParts(arg1:int):void
        {
            this._parts = arg1;
            return;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            var loc5:*=null;
            var loc9:*=null;
            var loc1:*=this._parts != -1 ? this._parts : 10 + 15 * _rnd.nextDouble();
            var loc2:*=_track.getLastSegment();
            if (arg1 is com.playchilla.runner.track.generator.CurveGenerator || arg1 is com.playchilla.runner.track.generator.LoopGenerator) 
            {
                addForwardSegment(getNextY(arg2), 0, 0, 3, arg3);
            }
            var loc3:*;
            var loc4:*=(loc3 = loc2 = _track.getLastSegment()).getParts();
            var loc6:*=(loc5 = _materials.getMaterialVector("warning")).length;
            var loc7:*=(loc4.length - 1);
            var loc8:*=1;
            while (loc8 < loc6) 
            {
                if (loc7 < 0) 
                {
                    if ((loc3 = loc3.getPreviousSegment()) == null || loc3 is com.playchilla.runner.track.segment.HoleSegment) 
                    {
                        break;
                    }
                    loc7 = ((loc4 = loc3.getParts()).length - 1);
                }
                (loc9 = loc4[loc7--]).mesh.material = loc5[loc6 - loc8];
                ++loc8;
            }
            _track.addSegment(new com.playchilla.runner.track.segment.HoleSegment(_track.getConnectPart(), _track.getConnectPart().dir, loc1, arg3));
            return;
        }

        internal var _parts:int=-1;
    }
}


// ============================================================================
// IslandGenerator - 岛屿生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class IslandGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function IslandGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            this._hg = new com.playchilla.runner.track.generator.HoleGenerator(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg3 < 20) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.02 + 0.06 * arg2) 
            {
                return false;
            }
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            var loc1:*=_rnd.nextIntRange(3, 3 + _rnd.nextDouble() * 6 * arg2);
            var loc2:*=0;
            while (loc2 < loc1) 
            {
                addForwardSegment(getNextY(arg2), 0, 0, _rnd.nextDouble() < 0.1 ? 4 : 7, arg3);
                this._hg.generate(this, arg2, arg3);
                ++loc2;
            }
            return;
        }

        internal var _hg:com.playchilla.runner.track.generator.HoleGenerator;
    }
}


// ============================================================================
// LongJumpGenerator - 长跳生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class LongJumpGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function LongJumpGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            this._hg = new com.playchilla.runner.track.generator.HoleGenerator(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg3 < 20) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.05) 
            {
                return false;
            }
            return true;
        }

        public function setHoleParts(arg1:int):void
        {
            this._holeParts = arg1;
            return;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            addForwardSegment(getNextY(arg2), 0, -35, 10, arg3);
            addForwardSegment(getNextY(arg2), 0, 35, 15, arg3);
            addForwardSegment(getNextY(arg2), 0, 35, 10, arg3);
            addForwardSegment(getNextY(arg2), 0, -55, 15, arg3);
            addForwardSegment(getNextY(arg2), 0, 0, 2, arg3);
            this._hg.setParts(this._holeParts != -1 ? this._holeParts : 50 + 30 * arg2);
            this._hg.generate(this, arg2, arg3);
            return;
        }

        internal var _hg:com.playchilla.runner.track.generator.HoleGenerator;

        internal var _holeParts:int=-1;
    }
}


// ============================================================================
// LongJumpTutorialGenerator - 长跳教程生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class LongJumpTutorialGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function LongJumpTutorialGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            this._longJumpGen = new com.playchilla.runner.track.generator.LongJumpGenerator(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            addForwardSegment(getNextY(arg2), 0, 0, 60, arg3);
            this._longJumpGen.generate(this, 0.85, arg3);
            addForwardSegment(getNextY(arg2), 0, 0, 20, arg3);
            return;
        }

        internal var _longJumpGen:com.playchilla.runner.track.generator.LongJumpGenerator;
    }
}


// ============================================================================
// LoopGenerator - 环形生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class LoopGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function LoopGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            return arg3 > 5 && _rnd.nextDouble() < 0.08 + 0.02 * arg2;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            var loc1:*=getNextY(arg2);
            var loc2:*=35 + _rnd.nextDouble() * 30;
            addForwardSegment(loc1, 10, -360, loc2, arg3);
            return;
        }
    }
}


// ============================================================================
// SlopeGenerator - 斜坡生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class SlopeGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function SlopeGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials, arg4:Boolean)
        {
            super(arg1, arg2, arg3);
            this._up = arg4;
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            if (arg2 < 0.3) 
            {
                return false;
            }
            if (_rnd.nextDouble() > 0.15) 
            {
                return false;
            }
            if (this._up && _track.getConnectPart().getPos().y > 190) 
            {
                return false;
            }
            if (!this._up && _track.getConnectPart().getPos().y < 110) 
            {
                return false;
            }
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            addForwardSegment(getNextY(arg2), 0, this._up ? -45 : 45, 5 + 15 * _rnd.nextDouble(), arg3);
            addForwardSegment(getNextY(arg2), 0, this._up ? 45 : -45, 5 + 15 * _rnd.nextDouble(), arg3);
            return;
        }

        internal var _up:Boolean;
    }
}


// ============================================================================
// TutorialGenerator - 教程生成器
// ============================================================================
package com.playchilla.runner.track.generator 
{
    import com.playchilla.runner.*;
    import com.playchilla.runner.track.*;
    import shared.math.*;
    
    public class TutorialGenerator extends com.playchilla.runner.track.generator.SegmentGenerator implements com.playchilla.runner.track.generator.ISegmentGenerator
    {
        public function TutorialGenerator(arg1:com.playchilla.runner.track.Track, arg2:shared.math.Random, arg3:com.playchilla.runner.Materials)
        {
            super(arg1, arg2, arg3);
            this._hg = new com.playchilla.runner.track.generator.HoleGenerator(arg1, arg2, arg3);
            this._loopGenerator = new com.playchilla.runner.track.generator.LoopGenerator(arg1, arg2, arg3);
            return;
        }

        public function canRun(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):Boolean
        {
            return true;
        }

        public function generate(arg1:com.playchilla.runner.track.generator.ISegmentGenerator, arg2:Number, arg3:int):void
        {
            if (this._step < 10) 
            {
                addForwardSegment(150, 0, 0, 20, arg3);
            }
            else if (this._step != 10) 
            {
                if (this._step != 11) 
                {
                    if (this._step != 12) 
                    {
                        if (this._step != 13) 
                        {
                            if (this._step != 14) 
                            {
                                if (this._step != 15) 
                                {
                                    if (this._step != 16) 
                                    {
                                        if (this._step == 17) 
                                        {
                                            addForwardSegment(60, 0, 0, 100, arg3);
                                        }
                                    }
                                    else 
                                    {
                                        this._loopGenerator.generate(arg1, arg2, arg3);
                                    }
                                }
                                else 
                                {
                                    addForwardSegment(60, 0, 0, 100, arg3);
                                }
                            }
                            else 
                            {
                                this._hg.generate(this, 0, arg3);
                            }
                        }
                        else 
                        {
                            addForwardSegment(150, 0, 0, 100, arg3);
                        }
                    }
                    else 
                    {
                        this._hg.generate(this, 0, arg3);
                    }
                }
                else 
                {
                    addForwardSegment(150, 0, 0, 50, arg3);
                }
            }
            else 
            {
                this._hg.generate(this, 0, arg3);
            }
            var loc1:*;
            var loc2:*=((loc1 = this)._step + 1);
            loc1._step = loc2;
            return;
        }

        internal var _hg:com.playchilla.runner.track.generator.HoleGenerator;

        internal var _loopGenerator:com.playchilla.runner.track.generator.LoopGenerator;

        internal var _step:int=0;
    }
}

// ============================================================================
// 文件结束
// ============================================================================
