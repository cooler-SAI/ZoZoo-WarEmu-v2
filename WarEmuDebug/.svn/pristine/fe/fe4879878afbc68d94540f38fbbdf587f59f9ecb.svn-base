using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarhammerEmu.GameServer
{
    public class Movement : IPacketHandler
    {
        enum MovementTypes
        {
            GroundForward = 0xC0,
            GroundBackward = 0x54,
            FlyModeForward = 0x88,
            FlyModeBackward = 0x00,
            NotMoving = 0xFE,

        }

        enum Strafe  // 
        {
            left1 = 0x20,
            left2 = 0xa0,

            leftforward1 = 0x30,
            leftforward2 = 0xb0,

            right1 = 0x40,
            right2 = 0xc0,

            rightforward1 = 0x50,
            rightforward2 = 0xd0,

            jumpright = 0x44,
            jumpleft = 0x24,
        }




        static public void GetPosition(UInt16 x, UInt16 y, UInt16 z)
        {
            int YOffset = 200 << 12;
            int XOffset = 200 << 12;

            int X = x + XOffset;
            int Y = y + YOffset;


            Log.Debug(string.Format("WorldPos: X = {0}  Y = {1} Z = {2}", X, Y, z));
            Log.Debug(string.Format("MapPos: X = {0}  Y = {1} Z = {2}", x, y, z));
        }
        [PacketHandlerAttribute((int)Opcodes.F_PLAYER_STATE2)]
        static public void F_PLAYER_STATE2(Connection conn, PacketIn packet)
        {
            Log.DumpHex(packet.ToArray());
            UInt16 unk1 = packet.GetUint16();
            byte Type = packet.GetUint8();
            byte MoveingState = packet.GetUint8(); // ground, jumpstart,jumpend,inair
            byte unk2 = packet.GetUint8();
            byte Strafe = packet.GetUint8();

            UInt16 Heading = packet.GetUint16R();
            UInt16 X = packet.GetUint16R();
            UInt16 Y = packet.GetUint16R();
            byte Unk1 = packet.GetUint8();
            UInt16 Z = packet.GetUint16R();

            X /= 2;
            Y /= 2;
            Z /= 4;

            
            if (packet.Size < 10)
            {
                Log.Debug("Player: state refresh");
            }
            else
            {
                if (Type == (byte)MovementTypes.NotMoving)
                {
                    // player not moving or has stopped in position
                    Log.Debug("Player: not moving or has stopped in position " + Type);
                }
                else if (Type == (byte)MovementTypes.GroundForward)
                {
                    // player is moving forward
                    Log.Debug("Player: moving forward " + Type);
                }
                else if (Type == (byte)MovementTypes.GroundBackward)
                {
                    // player is moving backward
                    Log.Debug("Player: moving backward " + Type);
                }
                else if (Type == (byte)MovementTypes.FlyModeForward)
                {
                    // player is using fly mode
                    Log.Debug("Player: flying forward " + Type);
                }
                else if (Type == (byte)MovementTypes.FlyModeBackward)
                {
                    // player is using fly mode
                    Log.Debug("Player: flying backward " + Type);
                }

                GetPosition(X, Y, Z);
                

                
            }
            /*try
            {
                long Pos = packet.Position;

                PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_STATE2);
                Out.Write(packet.ToArray(), (int)packet.Position, (int)packet.Size);
                Out.WriteByte(0);
               // Plr.DispatchPacket(Out, false);

                packet.Position = Pos;
            }
            catch (Exception e)
            {
                Log.Error("F_PLAYER_STATE2", e.ToString());
            }

            if (packet.Size < 18)
            {
                Log.Debug("Player: not moving");
                Log.DumpHex(packet.ToArray());
                return;
            }

            UInt16 Key = packet.GetUint16();

            byte MoveByte = packet.GetUint8();
            byte UnkByte = packet.GetUint8();
            byte CombatByte = packet.GetUint8();
            byte RotateByte = packet.GetUint8();

            UInt16 Heading = packet.GetUint16R();
            UInt16 X = packet.GetUint16R();
            UInt16 Y = packet.GetUint16R();
            byte Unk1 = packet.GetUint8();
            UInt16 Z = packet.GetUint16R();
            byte Unk2 = packet.GetUint8();
            Log.Debug(string.Format("Player: X={0} Y={1} Z={2}",X,Y,Z));*/
        }
    }
}
