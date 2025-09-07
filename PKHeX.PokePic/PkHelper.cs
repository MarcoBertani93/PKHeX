using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PKHeX.Core;
using XmlPictureCreation;
using XmlPictureCreation.Aliases;

namespace PKHeX.PokePic
{
    internal class PkHelper
    {
        public static VariabileCollection GetValues(PKM pk)
        {
            VariabileCollection variables = new();

            var str = GameInfo.Strings = GameInfo.GetStrings("it");

            // Look LocalizeUtil
            GameStrings localized_Strings;


            var species = pk.Species;

            variables.Add($"species", species);
            variables.Add($"generation", pk.Generation);
            variables.Add($"type1", pk.PersonalInfo.Type1);
            variables.Add($"type2", pk.PersonalInfo.Type2);
            variables.Add($"hasType2", pk.PersonalInfo.Type2 != pk.PersonalInfo.Type1);

            variables.Add($"isNicknamed", pk.IsNicknamed);
            variables.Add($"nickname", pk.Nickname);

            bool isMale = pk.Gender == 0;
            bool isFemale = pk.Gender == 1;
            bool isNeutral = pk.Gender == 2;
            variables.Add($"gender.isMale", isMale);
            variables.Add($"gender.isFemale", isFemale);
            variables.Add($"gender.isNeutral", isNeutral);
            variables.Add($"gender.letter", isMale ? "M" : isFemale ? "F" : "N");
            variables.Add($"gender.symbol", isMale ? "♂" : isFemale ? "♀" : " ");

            variables.Add($"isShiny", pk.IsShiny);
            variables.Add($"hasPokerus", pk.IsPokerusInfected);
            variables.Add($"hadPokerus", pk.IsPokerusCured);
            variables.Add($"hasHeldItem", pk.HeldItem != 0);
            variables.Add($"heldItem", pk.HeldItem);
            variables.Add($"ball", pk.Ball);
            variables.Add($"friendship", pk.CurrentFriendship);

            var natureAmps = NatureAmp.GetAmps(pk.Nature);

            variables.Add($"stats.inc.atk", natureAmps[0] > 0);
            variables.Add($"stats.inc.def", natureAmps[1] > 0);
            variables.Add($"stats.inc.spe", natureAmps[2] > 0);
            variables.Add($"stats.inc.spa", natureAmps[3] > 0);
            variables.Add($"stats.inc.spd", natureAmps[4] > 0);

            variables.Add($"stats.dec.atk", natureAmps[0] < 0);
            variables.Add($"stats.dec.def", natureAmps[1] < 0);
            variables.Add($"stats.dec.spe", natureAmps[2] < 0);
            variables.Add($"stats.dec.spa", natureAmps[3] < 0);
            variables.Add($"stats.dec.spd", natureAmps[4] < 0);

            variables.Add($"stats.base", pk.PersonalInfo.GetBaseStatTotal());
            variables.Add($"stats.base.hp", pk.PersonalInfo.HP);
            variables.Add($"stats.base.atk", pk.PersonalInfo.ATK);
            variables.Add($"stats.base.def", pk.PersonalInfo.DEF);
            variables.Add($"stats.base.spe", pk.PersonalInfo.SPE);
            variables.Add($"stats.base.spa", pk.PersonalInfo.SPA);
            variables.Add($"stats.base.spd", pk.PersonalInfo.SPD);

            variables.Add($"stats.iv", pk.IVTotal);
            variables.Add($"stats.iv.hp", pk.IV_HP);
            variables.Add($"stats.iv.atk", pk.IV_ATK);
            variables.Add($"stats.iv.def", pk.IV_DEF);
            variables.Add($"stats.iv.spe", pk.IV_SPE);
            variables.Add($"stats.iv.spa", pk.IV_SPA);
            variables.Add($"stats.iv.spd", pk.IV_SPD);

            variables.Add($"stats.ev", pk.EVTotal);
            variables.Add($"stats.ev.hp", pk.EV_HP);
            variables.Add($"stats.ev.atk", pk.EV_ATK);
            variables.Add($"stats.ev.def", pk.EV_DEF);
            variables.Add($"stats.ev.spe", pk.EV_SPE);
            variables.Add($"stats.ev.spa", pk.EV_SPA);
            variables.Add($"stats.ev.spd", pk.EV_SPD);

            variables.Add($"stats.val.hp", pk.Stat_HPMax);
            variables.Add($"stats.val.atk", pk.Stat_ATK);
            variables.Add($"stats.val.def", pk.Stat_DEF);
            variables.Add($"stats.val.spe", pk.Stat_SPE);
            variables.Add($"stats.val.spa", pk.Stat_SPA);
            variables.Add($"stats.val.spd", pk.Stat_SPD);

            variables.Add($"moves", pk.MoveCount);
            variables.Add($"move1", pk.MoveCount >= 1);
            variables.Add($"move2", pk.MoveCount >= 2);
            variables.Add($"move3", pk.MoveCount >= 3);
            variables.Add($"move4", pk.MoveCount >= 4);

            variables.Add($"move1.pp", pk.Move1_PP);
            variables.Add($"move2.pp", pk.Move2_PP);
            variables.Add($"move3.pp", pk.Move3_PP);
            variables.Add($"move4.pp", pk.Move4_PP);

            variables.Add($"move1.ppups", pk.Move1_PPUps);
            variables.Add($"move2.ppups", pk.Move2_PPUps);
            variables.Add($"move3.ppups", pk.Move3_PPUps);
            variables.Add($"move4.ppups", pk.Move4_PPUps);


            AddCurrentLocaleVariables(pk, variables);
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.Japanese));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.English));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.French));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.Italian));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.German));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.Spanish));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.Korean));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.ChineseS));
            AddLocalizedVariables(pk, variables, Language.GetLanguageCode(LanguageID.ChineseT));

#if DEBUG
            Dictionary<string, object> dict = new Dictionary<string, object>();

            variables.GetBooleans().ToList().ForEach(v => dict.Add(v.Id!, v.Value!));
            variables.GetNumbers().ToList().ForEach(v => dict.Add(v.Id!, v.Value!));
            variables.GetStrings().ToList().ForEach(v => dict.Add(v.Id!, v.Value!));

            var serialized = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
#endif

            return variables;
        }

        private static void AddCurrentLocaleVariables(PKM pk, VariabileCollection variables)
        {
            var localized_Strings = GameInfo.GetStrings(Language.GetLanguageCode(LanguageID.English));
            variables.Add($"species.name", localized_Strings.specieslist[pk.Species]);
            variables.Add($"type1.name", localized_Strings.types[pk.PersonalInfo.Type1]);
            variables.Add($"type2.name", localized_Strings.types[pk.PersonalInfo.Type2]);
            variables.Add($"move1.name", localized_Strings.movelist[pk.Move1]);
            variables.Add($"move2.name", localized_Strings.movelist[pk.Move2]);
            variables.Add($"move3.name", localized_Strings.movelist[pk.Move3]);
            variables.Add($"move4.name", localized_Strings.movelist[pk.Move4]);
            variables.Add($"move1.type", localized_Strings.types[MoveInfo.GetType(pk.Move1, pk.Context)]);
            variables.Add($"move2.type", localized_Strings.types[MoveInfo.GetType(pk.Move2, pk.Context)]);
            variables.Add($"move3.type", localized_Strings.types[MoveInfo.GetType(pk.Move3, pk.Context)]);
            variables.Add($"move4.type", localized_Strings.types[MoveInfo.GetType(pk.Move4, pk.Context)]);
            variables.Add($"ability.name", localized_Strings.abilitylist[pk.Ability]);
            variables.Add($"nature.name", localized_Strings.Natures[(int)pk.Nature]);
            variables.Add($"statnature.name", localized_Strings.Natures[(int)pk.StatNature]);
            variables.Add($"heldItem.name", localized_Strings.itemlist[pk.HeldItem]);
            variables.Add($"ball.name", localized_Strings.balllist[pk.Ball]);
        }

        private static void AddLocalizedVariables(PKM pk, VariabileCollection variables, string lang)
        {
            var localized_Strings = GameInfo.GetStrings(lang);
            variables.Add($"species.name.{lang}", localized_Strings.specieslist[pk.Species]);
            variables.Add($"type1.name.{lang}", localized_Strings.types[pk.PersonalInfo.Type1]);
            variables.Add($"type2.name.{lang}", localized_Strings.types[pk.PersonalInfo.Type2]);
            variables.Add($"move1.name.{lang}", localized_Strings.movelist[pk.Move1]);
            variables.Add($"move2.name.{lang}", localized_Strings.movelist[pk.Move2]);
            variables.Add($"move3.name.{lang}", localized_Strings.movelist[pk.Move3]);
            variables.Add($"move4.name.{lang}", localized_Strings.movelist[pk.Move4]);
            variables.Add($"move1.type.{lang}", localized_Strings.types[MoveInfo.GetType(pk.Move1, pk.Context)]);
            variables.Add($"move2.type.{lang}", localized_Strings.types[MoveInfo.GetType(pk.Move2, pk.Context)]);
            variables.Add($"move3.type.{lang}", localized_Strings.types[MoveInfo.GetType(pk.Move3, pk.Context)]);
            variables.Add($"move4.type.{lang}", localized_Strings.types[MoveInfo.GetType(pk.Move4, pk.Context)]);
            variables.Add($"ability.name.{lang}", localized_Strings.abilitylist[pk.Ability]);
            variables.Add($"nature.name.{lang}", localized_Strings.Natures[(int)pk.Nature]);
            variables.Add($"statnature.name.{lang}", localized_Strings.Natures[(int)pk.StatNature]);
            variables.Add($"heldItem.name.{lang}", localized_Strings.itemlist[pk.HeldItem]);
            variables.Add($"ball.name.{lang}", localized_Strings.balllist[pk.Ball]);
        }


    }

}

