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
using XmlPictureCreation.Objects.Interfaces;

namespace PKHeX.PokePic.Helpers
{
    internal class PkmHelper
    {
        private static readonly Assembly? pokeSpriteAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "PKHeX.Drawing.PokeSprite");

        public static VariableCollection GetVariables(PKM pkm)
        {
            VariableCollection variables = [];

            variables.Add($"system.language", GameInfo.CurrentLanguage);    // Current app languge: "en", "it", "jp", ... 

            variables.Add($"species", pkm.Species);
            variables.Add($"level", pkm.CurrentLevel);
            variables.Add($"generation", pkm.Generation);
            variables.Add($"type1", pkm.PersonalInfo.Type1);
            variables.Add($"type2", pkm.PersonalInfo.Type2);
            variables.Add($"hasType2", pkm.PersonalInfo.Type2 != pkm.PersonalInfo.Type1);

            variables.Add($"isNicknamed", pkm.IsNicknamed);
            variables.Add($"nickname", pkm.Nickname);

            bool isMale = pkm.Gender == 0;
            bool isFemale = pkm.Gender == 1;
            bool isNeutral = pkm.Gender == 2;
            variables.Add($"gender.isMale", isMale);
            variables.Add($"gender.isFemale", isFemale);
            variables.Add($"gender.isNeutral", isNeutral);
            variables.Add($"gender.letter", isMale ? "M" : isFemale ? "F" : "N");
            variables.Add($"gender.symbol", isMale ? "♂" : isFemale ? "♀" : " ");

            variables.Add($"isShiny", pkm.IsShiny);
            variables.Add($"hasPokerus", pkm.IsPokerusInfected);
            variables.Add($"hadPokerus", pkm.IsPokerusCured);
            variables.Add($"hasHeldItem", pkm.HeldItem != 0);
            variables.Add($"heldItem", pkm.HeldItem);
            variables.Add($"ball", pkm.Ball);
            variables.Add($"friendship", pkm.CurrentFriendship);

            var natureAmps = NatureAmp.GetAmps(pkm.Nature);

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

            variables.Add($"stats.base", pkm.PersonalInfo.GetBaseStatTotal());
            variables.Add($"stats.base.hp", pkm.PersonalInfo.HP);
            variables.Add($"stats.base.atk", pkm.PersonalInfo.ATK);
            variables.Add($"stats.base.def", pkm.PersonalInfo.DEF);
            variables.Add($"stats.base.spe", pkm.PersonalInfo.SPE);
            variables.Add($"stats.base.spa", pkm.PersonalInfo.SPA);
            variables.Add($"stats.base.spd", pkm.PersonalInfo.SPD);

            variables.Add($"stats.iv", pkm.IVTotal);
            variables.Add($"stats.iv.hp", pkm.IV_HP);
            variables.Add($"stats.iv.atk", pkm.IV_ATK);
            variables.Add($"stats.iv.def", pkm.IV_DEF);
            variables.Add($"stats.iv.spe", pkm.IV_SPE);
            variables.Add($"stats.iv.spa", pkm.IV_SPA);
            variables.Add($"stats.iv.spd", pkm.IV_SPD);

            variables.Add($"stats.ev", pkm.EVTotal);
            variables.Add($"stats.ev.hp", pkm.EV_HP);
            variables.Add($"stats.ev.atk", pkm.EV_ATK);
            variables.Add($"stats.ev.def", pkm.EV_DEF);
            variables.Add($"stats.ev.spe", pkm.EV_SPE);
            variables.Add($"stats.ev.spa", pkm.EV_SPA);
            variables.Add($"stats.ev.spd", pkm.EV_SPD);

            variables.Add($"stats.val.hp", pkm.Stat_HPMax);
            variables.Add($"stats.val.atk", pkm.Stat_ATK);
            variables.Add($"stats.val.def", pkm.Stat_DEF);
            variables.Add($"stats.val.spe", pkm.Stat_SPE);
            variables.Add($"stats.val.spa", pkm.Stat_SPA);
            variables.Add($"stats.val.spd", pkm.Stat_SPD);

            variables.Add($"moves", pkm.MoveCount);
            variables.Add($"hasMove1", pkm.MoveCount >= 1);
            variables.Add($"hasMove2", pkm.MoveCount >= 2);
            variables.Add($"hasMove3", pkm.MoveCount >= 3);
            variables.Add($"hasMove4", pkm.MoveCount >= 4);

            variables.Add($"move1.type", MoveInfo.GetType(pkm.Move1, pkm.Context));
            variables.Add($"move2.type", MoveInfo.GetType(pkm.Move2, pkm.Context));
            variables.Add($"move3.type", MoveInfo.GetType(pkm.Move3, pkm.Context));
            variables.Add($"move4.type", MoveInfo.GetType(pkm.Move4, pkm.Context));

            variables.Add($"move1.pp", pkm.Move1_PP);
            variables.Add($"move2.pp", pkm.Move2_PP);
            variables.Add($"move3.pp", pkm.Move3_PP);
            variables.Add($"move4.pp", pkm.Move4_PP);

            variables.Add($"move1.ppups", pkm.Move1_PPUps);
            variables.Add($"move2.ppups", pkm.Move2_PPUps);
            variables.Add($"move3.ppups", pkm.Move3_PPUps);
            variables.Add($"move4.ppups", pkm.Move4_PPUps);


            AddCurrentLocaleVariables(pkm, variables);
            AddLocalizedVariables(pkm, variables, LanguageID.Japanese.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.English.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.French.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.Italian.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.German.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.Spanish.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.Korean.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.ChineseS.GetLanguageCode());
            AddLocalizedVariables(pkm, variables, LanguageID.ChineseT.GetLanguageCode());

            var getTypeSpriteColor = pokeSpriteAssembly?.GetTypes()
              .Where(t => t.IsSealed && t.IsAbstract) // class statica
              .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
              .Where(m => m.Name == "GetTypeSpriteColor")
              .FirstOrDefault(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(byte));

            if (getTypeSpriteColor is not null)
            {
                var color1 = (Color)getTypeSpriteColor?.Invoke(null, [pkm.PersonalInfo.Type1])!;
                variables.Add("type1.color", color1);
                var color2 = (Color)getTypeSpriteColor?.Invoke(null, [pkm.PersonalInfo.Type2])!;
                variables.Add("type2.color", color2);

                var move1color = (Color)getTypeSpriteColor?.Invoke(null, [MoveInfo.GetType(pkm.Move1, pkm.Context)])!;
                variables.Add("move1.type.color", move1color);
                var move2color = (Color)getTypeSpriteColor?.Invoke(null, [MoveInfo.GetType(pkm.Move2, pkm.Context)])!;
                variables.Add("move2.type.color", move2color);
                var move3color = (Color)getTypeSpriteColor?.Invoke(null, [MoveInfo.GetType(pkm.Move3, pkm.Context)])!;
                variables.Add("move3.type.color", move3color);
                var move4color = (Color)getTypeSpriteColor?.Invoke(null, [MoveInfo.GetType(pkm.Move4, pkm.Context)])!;
                variables.Add("move4.type.color", move4color);
            }
            return variables;
        }

        private static void AddCurrentLocaleVariables(PKM pkm, VariableCollection variables)
        {
            var localized_Strings = GameInfo.GetStrings(LanguageID.English.GetLanguageCode());
            variables.Add($"species.name", localized_Strings.specieslist[pkm.Species]);
            variables.Add($"type1.name", localized_Strings.types[pkm.PersonalInfo.Type1]);
            variables.Add($"type2.name", localized_Strings.types[pkm.PersonalInfo.Type2]);
            variables.Add($"move1.name", localized_Strings.movelist[pkm.Move1]);
            variables.Add($"move2.name", localized_Strings.movelist[pkm.Move2]);
            variables.Add($"move3.name", localized_Strings.movelist[pkm.Move3]);
            variables.Add($"move4.name", localized_Strings.movelist[pkm.Move4]);
            variables.Add($"move1.type.name", localized_Strings.types[MoveInfo.GetType(pkm.Move1, pkm.Context)]);
            variables.Add($"move2.type.name", localized_Strings.types[MoveInfo.GetType(pkm.Move2, pkm.Context)]);
            variables.Add($"move3.type.name", localized_Strings.types[MoveInfo.GetType(pkm.Move3, pkm.Context)]);
            variables.Add($"move4.type.name", localized_Strings.types[MoveInfo.GetType(pkm.Move4, pkm.Context)]);
            variables.Add($"ability.name", localized_Strings.abilitylist[pkm.Ability]);
            variables.Add($"nature.name", localized_Strings.Natures[(int)pkm.Nature]);
            variables.Add($"statnature.name", localized_Strings.Natures[(int)pkm.StatNature]);
            variables.Add($"heldItem.name", localized_Strings.itemlist[pkm.HeldItem]);
            variables.Add($"ball.name", localized_Strings.balllist[pkm.Ball]);
        }

        private static void AddLocalizedVariables(PKM pkm, VariableCollection variables, string lang)
        {
            var localized_Strings = GameInfo.GetStrings(lang);
            variables.Add($"species.name.{lang}", localized_Strings.specieslist[pkm.Species]);
            variables.Add($"type1.name.{lang}", localized_Strings.types[pkm.PersonalInfo.Type1]);
            variables.Add($"type2.name.{lang}", localized_Strings.types[pkm.PersonalInfo.Type2]);
            variables.Add($"move1.name.{lang}", localized_Strings.movelist[pkm.Move1]);
            variables.Add($"move2.name.{lang}", localized_Strings.movelist[pkm.Move2]);
            variables.Add($"move3.name.{lang}", localized_Strings.movelist[pkm.Move3]);
            variables.Add($"move4.name.{lang}", localized_Strings.movelist[pkm.Move4]);
            variables.Add($"move1.type.name.{lang}", localized_Strings.types[MoveInfo.GetType(pkm.Move1, pkm.Context)]);
            variables.Add($"move2.type.name.{lang}", localized_Strings.types[MoveInfo.GetType(pkm.Move2, pkm.Context)]);
            variables.Add($"move3.type.name.{lang}", localized_Strings.types[MoveInfo.GetType(pkm.Move3, pkm.Context)]);
            variables.Add($"move4.type.name.{lang}", localized_Strings.types[MoveInfo.GetType(pkm.Move4, pkm.Context)]);
            variables.Add($"ability.name.{lang}", localized_Strings.abilitylist[pkm.Ability]);
            variables.Add($"nature.name.{lang}", localized_Strings.Natures[(int)pkm.Nature]);
            variables.Add($"statnature.name.{lang}", localized_Strings.Natures[(int)pkm.StatNature]);
            variables.Add($"heldItem.name.{lang}", localized_Strings.itemlist[pkm.HeldItem]);
            variables.Add($"ball.name.{lang}", localized_Strings.balllist[pkm.Ball]);
        }

        public static ImageDictionary GetImages(PKM pkm)
        {
            var images = new ImageDictionary();
            // Try and get the current sprite by loading the PokeSprite assembly that contains the extension method for the sprite:

            if (pokeSpriteAssembly is null)
                return images;

            var getSprite = pokeSpriteAssembly?.GetTypes()
                .Where(t => t.IsSealed && t.IsAbstract) // class statica
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .FirstOrDefault(m => m.Name == "Sprite"
                                  && m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false));

            if (getSprite is not null)
            {
                var sprite = (Bitmap)getSprite?.Invoke(null, [pkm])!;
                images.Add("species.sprite", sprite);
            }

            var getBallSprite = pokeSpriteAssembly?.GetTypes()
               .Where(t => t.IsSealed && t.IsAbstract) // class statica
               .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
               .FirstOrDefault(m => m.Name == "GetBallSprite");

            if (getBallSprite is not null)
            {
                var sprite = (Bitmap)getBallSprite?.Invoke(null, [pkm.Ball])!;
                images.Add("ball.sprite", sprite);
            }

            var getItemSprite = pokeSpriteAssembly?.GetTypes()
               .Where(t => t.IsSealed && t.IsAbstract) // class statica
               .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
               .FirstOrDefault(m => m.Name == "GetItemSprite");

            if (getItemSprite is not null)
            {
                var sprite = (Bitmap)getItemSprite?.Invoke(null, [pkm.HeldItem])!;
                images.Add("heldItem.sprite", sprite);
            }

            return images;
        }
    }

}

