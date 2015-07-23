using System;
using System.Collections.Generic;
//APP STORE LINK https://itunes.apple.com/us/app/super-jelly-troopers/id925880432?ls=1&mt=8
namespace RescueJelly
{
		public class GameConstants
		{
				//public GameConstants ()
				//{
				//}
				public const string PLAYING_WORLD = "last_world";
				public const string PLAYING_LEVEL = "last_level";

				public const float WIDESCREEN_CORRECTION_VALUE = 64f;

				public const string SPIKES_LINE_BARRIER = "SpikesLine";

				//the normal hitpoints for a player
				public const int NUM_LIFES_PER_LEVEL = 4;

				public const int NUM_LEVELS_PER_WORLD = 6;
				public const int NUM_WORLDS = 4;

				//THIS ARE JUST FOR THE CURRENT LEVEL
				//number of seconds left when finished the 
				public const string LEVEL_REMAINING_TIME_SECS = "remaining_level_time_secs";
				public const string LEVEL_REMAINING_LIFES = "remaining_lifes";
				public const string LEVEL_SAVED_JELLIES = "saved_jellies";

				public const string LEVEL_REMAINING_TIME_SECS_SCORE = "remaining_level_time_secs_score";
				public const string LEVEL_REMAINING_LIFES_SCORE = "remaining_lifes_score";
				public const string LEVEL_SAVED_JELLIES_SCORE = "saved_jellies_score";


				//for the game resume on game over
				public const string CURRENT_WORLD_KEY = "CurrentWorld";
				public const string CURRENT_LEVEL_KEY = "CurrentLevel";


				//achievements same ids of GAME CENTER
				public const string ACHIEVEMENT_GURU_KEY = "rescued_all_jelly_troopers";
				public const string ACHIEVEMENT_HERO_KEY = "rescued_150_jelly_troopers";
				public const string ACHIEVEMENT_LEGEND_KEY = "rescued_200_jelly_troopers";
				public const string ACHIEVEMENT_BRAVE_KEY = "rescued_110_jelly_troopers"; //typo on game center, should be 100 but is 110

				//achievements points
				public const int ACHIEVEMENT_LEGEND_CHECKPOINT = 200; //saved 200 troopers
				public const int ACHIEVEMENT_HERO_CHECKPOINT = 150; //saved 150 troopers
				public const int ACHIEVEMENT_BRAVE_CHECKPOINT = 100; //saved 100 troopers
				//public const int ACHIEVEMENT_GURU_CHECKPOINT = END GAME :-)

				//leaderboards
				public const string LEADERBOARD_MAIN_SCORE = "jelly_troopers_main_leaderboard";
				public const string LEADERBOARD_BEST_TIME = "jelly_troopers_best_time_leaderboard";
				public const string LEADERBOARD_LESS_DEATHS = "jelly_troopers_less_deaths_leaderboard";

				//local keys, for the leaderboards 
				//( THESE ARE GLOBALS/ENTIRE GAMEPLAY KEYS )
				public const string TOTAL_ELAPSED_TIME_SECS_KEY = "total_elapsed_time_secs";
				public const string TOTAL_LOST_LIFES_KEY = "total_lost_lifes";
				public const string TOTAL_SAVED_JELLIES_KEY = "total_saved_jellies";
				public const string TOTAL_SCORE_KEY = "total_score";
				public const string HIGH_SCORE_KEY = "high_score";


				//missions
				public const string MISSION_1_KEY = "mission_1";
				public const string MISSION_2_KEY = "mission_2";
				public const string MISSION_3_KEY = "mission_3";
				public const string MISSION_4_KEY = "mission_4";


				//this is the world clicked on missions screen (re-writen each time)
				public const string MISSION_SELECT_WORLD_SELECTED_KEY = "mission_world";
				//missions / levels already achieved (for each world), this is never deleted
				//level 1 is always unlocked in all achieved worlds
				//these are concatened with the missions keys above
				public const string MISSION_SELECT_LEVEL_TWO_KEY = "_level_2";
				public const string MISSION_SELECT_LEVEL_THREE_KEY = "_level_3";
				public const string MISSION_SELECT_LEVEL_FOUR_KEY = "_level_4";
				public const string MISSION_SELECT_LEVEL_FIVE_KEY = "_level_5";
				public const string MISSION_SELECT_LEVEL_SIX_KEY = "_level_6";

				//translation keys
				public const string MSG_HOW_TO_PLAY="how_to_play";
				public const string MSG_HOW_TO_PLAY_LAST_LEVEL="how_to_play_last_level";
				public const string MSG_TAP_TROOPER="tap_trooper";
				public const string MSG_TAP_LEFT_RIGHT="tap_left_right";
				public const string MSG_LAND_ALL="land_all_safely";
				public const string MSG_RESCUED="rescued";
				public const string MSG_LIFES="lifes";
				public const string MSG_INFINITE_LIFES="infinite_lifes";
				public const string MSG_TIME="time";
				public const string MSG_WORLD="world";
				public const string MSG_LEVEL="level";
				public const string MSG_HURRY_UP="hurry_up";
				public const string MSG_FAILSAFE="failsafe";
				public const string MSG_CONGRATULATIONS="congratulations";
				public const string MSG_NEXT="next";
				public const string MSG_HIGH_SCORE="high_sore";
				public const string MSG_HELPS_BOOSTERS="helps";
				public const string MSG_GET_SOME_ACTION="msg_action";
				public const string MSG_GET_LETS_DOIT="msg_doit";
				public const string MSG_GAME_CENTER_ERROR="game_center_error";
				public const string MSG_INTRUSION_ALERT="intrusion_alert";
				public const string MSG_TAP_UNLOCKED_MISSION = "tap_mission_unlock";


				//IN-App
				public const string MSG_BUY_EXTRA_LIFES="buy_extra_lifes";
				public const string MSG_BUY_EXTRA_TIME="buy_extra_time";
				public const string MSG_BUY_EXTRA_SPEED="buy_extra_speed";
				public const string MSG_BUY_INFINITE_LIFES="buy_infinite_lifes";

				//in app purchases
				public const int 	IN_APP_PURCHASE_EXTRA_TIME_IN_SECONDS = 30; //extra 30 seconds
				public const int 	IN_APP_PURCHASE_EXTRA_LIFES_COUNT = 2; //2 extra lifes
				public const int 	IN_APP_PURCHASE_INFINITE_LIFES_COUNT = 1000; //1000 extra lifes, still dies because of time
				public const float 	IN_APP_PURCHASE_EXTRA_SPEED_INCREASE_FACTOR = 2.5f;//double the speed in x axis

				//#if UNITY_ANDROID && !UNITY_EDITOR


				public static Dictionary<string, string> ANDROID_DICTIONARY = new Dictionary<string, string>
		    	{
						{"jelly_troopers_main_leaderboard", "CgkI7La6roAbEAIQBg"},
						{"jelly_troopers_best_time_leaderboard", "CgkI7La6roAbEAIQBw"},
						{"jelly_troopers_less_deaths_leaderboard", "CgkI7La6roAbEAIQCA"},
						{"rescued_all_jelly_troopers", "CgkI7La6roAbEAIQAQ"},
						{"rescued_150_jelly_troopers", "CgkI7La6roAbEAIQAg"},
						{"rescued_200_jelly_troopers", "CgkI7La6roAbEAIQAw"},
						{"rescued_110_jelly_troopers", "CgkI7La6roAbEAIQBA"}
		    	};
				//#endif
		}
}

