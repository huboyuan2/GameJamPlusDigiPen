/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID GAME_BOOT = 4201676062U;
        static const AkUniqueID PLAY_SHOTGUN_FIRE = 2682934687U;
        static const AkUniqueID PLAY_SHOTGUN_JUMP = 2277574539U;
        static const AkUniqueID PLAYERDIED = 1886223524U;
        static const AkUniqueID PLAYERHURT = 3537581393U;
        static const AkUniqueID SET_PLAYER_DEAD = 136861954U;
        static const AkUniqueID SET_PLAYER_NOT_DEAD = 3892237736U;
        static const AkUniqueID SET_STATE_GAME = 4207344442U;
        static const AkUniqueID SET_STATE_NORMAL = 2630068541U;
        static const AkUniqueID SET_STATE_SPECIAL = 2058646701U;
        static const AkUniqueID SET_STATE_TITLE = 2875859334U;
        static const AkUniqueID TO_GAMEPLAY = 1061087905U;
        static const AkUniqueID TO_TITLE = 1465864013U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GAME_FSM
        {
            static const AkUniqueID GROUP = 3589896044U;

            namespace STATE
            {
                static const AkUniqueID GAME = 702482391U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID TITLE = 3705726509U;
            } // namespace STATE
        } // namespace GAME_FSM

        namespace GAME_STATE
        {
            static const AkUniqueID GROUP = 766723505U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID NORMAL = 1160234136U;
                static const AkUniqueID SPECIAL = 3064974266U;
            } // namespace STATE
        } // namespace GAME_STATE

    } // namespace STATES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID MUSIC_PLAYRATE = 4166321325U;
        static const AkUniqueID VOLUME_MUS = 3628477563U;
        static const AkUniqueID VOLUME_SFX = 3673881719U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUS = 712897226U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
