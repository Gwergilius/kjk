# Fighting Fantasy Card Art Project

## Projekt kontextus
Kalandjáték karaktergeneráló kártyák Midjourney illusztrációi.
Stílus: Bob Harvey pen-and-ink, color wash, Fighting Fantasy gamebook art.
Paraméterek: --ar 2:3 --sw 150 --style raw

## Mappastruktúra

```
./background/   - kártya hátlapok (KÉSZ)
./frames/       - kártyakeretek (KÉSZ)
./skill/        - skill kártyák (KÉSZ)
./stamina/      - elkészült stamina kártyák
./luck/         - elkészült luck kártyák (KÉSZ)
./draft/        - legutolsó prompt eredményei (aktív munka)
```

## Munkafolyamat
1. Claude Code generálja a Midjourney promptot
2. Felhasználó futtatja Midjourney-ben
3. Eredmény képek → `./draft/`
4. Claude Code értékeli a draft képeket és javasol befutót
5. Kiválasztott kép → `./stamina/` vagy `./luck/`

## Kártyasorozatok
- **Skill** (6 lap): hős figurák, értékek 7-12 — KÉSZ (monokróm színes)
- **Luck** (6 lap): ékkövek, értékek 7-12 — KÉSZ
- **Stamina** (11 lap): állatok, értékek 14-24 — KÉSZ (monokróm színes)
- **Potion** (3 lap): bájitalok — KÉSZ (színes, nem monokróm)
- **Redraw** (1 lap): rúnás kockák újraosztáshoz — KÉSZ (lila + arany)

## Stamina állatok
| Érték | Szimbólum |
|------:|-----------|
| 14 | The Rat |
| 15 | The Snake |
| 16 | The Fox |
| 17 | The Golden Eagle |
| 18 | The Boar |
| 19 | The Wolf |
| 20 | The Polar Bear |
| 21 | The Bull |
| 22 | The Gorilla |
| 23 | The Lion |
| 24 | The Dragon |

## Luck ékkövek
| Érték | Szimbólum |
|------:|-----------|
| 7 | The Opal |
| 8 | The Amethyst |
| 9 | The Sapphire |
| 10 | The Ruby |
| 11 | The Emerald |
| 12 | The Diamond |

## Skill kártyák — color assignment

Minden kártyához egyedi monokróm szín. Az indoklás: karakterhez illő hangulat + a hat kártya együtt széles spektrumot fedjen le.

| Érték | Karakter | Szín | Indoklás |
|------:|----------|------|----------|
| 7 | The Fool | Smaragdzöld | Természet, naivitás, kaland |
| 8 | The Thief | Éjkék (midnight blue) | Éjszaka, árnyék, titokzatosság |
| 9 | The Guard | Meleg amber/okker | Kő, tűz, hűség, várfal |
| 10 | The Champion | Vörös (crimson) | Csata, vér, dicsőség |
| 11 | The Knight | Királylila (royal purple) | Nemesség, lovagiasság |
| 12 | The Paladin | Arany (warm gold) | Szentség, isteni fény, erény |

## Prompt sablon — skill kártyák (monokróm színes)

```
Single fantasy card illustration, monochrome [COLOR NAME],
single color illustration, all tones in shades of [COLOR DESCRIPTION],
dark [SHADOW COLOR] shadows and pale [HIGHLIGHT COLOR] highlights, no other colors,
high contrast pen and ink, tarot card style composition,
no frame, no border, frameless, borderless, no card border, no decorative edge,
full bleed illustration filling the entire card area,
[CHARACTER DESCRIPTION],
[BACKGROUND DESCRIPTION],
detailed stippling and crosshatching, Bob Harvey art style,
Fighting Fantasy gamebook interior art,
no text, no numbers, no letters anywhere
--ar 2:3 --sw 150 --style raw
```

## Prompt sablon — ékkövek

```
Single fantasy card illustration, rich color illustration,
pen and ink with vibrant color wash,
tarot card style composition, no frame, no border, frameless, borderless, no card border, no decorative edge,
full bleed illustration filling the entire card area,
large single [GEM] gemstone as central subject,
[GEM-SPECIFIC COLOR AND TEXTURE],
highly detailed gem with realistic facets and light reflections,
internal light refracting through crystal,
dramatic light rays emanating from within the stone,
gem resting on dark velvet cloth surface,
neutral dark background with subtle radial light,
gem fills most of the card area, monumental presence,
no figures, no hands, no jewelry setting,
detailed stippling and crosshatching, Bob Harvey art style,
Fighting Fantasy gamebook interior art,
no text, no numbers, no letters anywhere
--ar 2:3 --sw 150 --style raw
```

## Stamina kártyák — color assignment

Minden állathoz egyedi monokróm szín. A szín az állat természetéhez, élőhelyéhez és hangulatához illeszkedik.

| Érték | Állat | Szín | Indoklás |
|------:|-------|------|----------|
| 14 | The Rat | Szépia (sepia) | Piszok, pince, alvilág |
| 15 | The Snake | Mérgező teal | Hidegvérű, veszélyes, ősi |
| 16 | The Fox | Réz/rozsda (burnt copper) | Ravaszság, ősz, melegség |
| 17 | The Golden Eagle | Égkék (cerulean blue) | Szabadság, magasság, éles tekintet, griff-szerű fenség |
| 18 | The Boar | Sötét rozsdabarna (raw sienna) | Vad, erdei, durva páncélbőr |
| 19 | The Wolf | Acélszürke (cold steel grey) | Falka, tél, éjszaka |
| 20 | The Polar Bear | Jeges fehér (arctic white) | Sarki fenség, legyőzhetetlen erő, magány |
| 21 | The Bull | Mély gesztenye (deep chestnut) | Vad erő, törtelen lendület |
| 22 | The Gorilla | Szénszürke (charcoal) | Nyers fizikai erő, fenyegető intelligencia |
| 23 | The Lion | Arany okker (sunlit ochre) | Királyság, nap, szavanna |
| 24 | The Dragon | Tűzvörös (deep vermillion) | Tűz, legfőbb hatalom, legenda |

## Prompt sablon — állatok (Stamina, monokróm színes)

```
Single fantasy card illustration, monochrome [COLOR NAME],
single color illustration, all tones in shades of [COLOR DESCRIPTION],
dark [SHADOW COLOR] shadows and pale [HIGHLIGHT COLOR] highlights, no other colors,
high contrast pen and ink,
tarot card style composition, no frame, no border, frameless, borderless, no card border, no decorative edge,
full bleed illustration filling the entire card area,
[ANIMAL DESCRIPTION AND POSE],
stylized decorative background: [HABITAT],
dramatic lighting, sense of [MOOD],
dense stippling and crosshatching throughout, Bob Harvey art style,
Fighting Fantasy gamebook interior art,
no text, no numbers, no letters anywhere
--ar 2:3 --sw 150 --style raw
```

## Tanulságok
- `young hero` → gyereket generál, használj `man in his mid-twenties`-t
- `halberd` → zavaros fegyvert generál, írd le fizikailag
- `dynamic sneaking posture` → levegőbe repíti a karaktert
- `no freestanding sword` → mindig add hozzá kardos jeleneteknél
- Ékkövek fekete-fehérben nem különböztethetők meg → színes generálás kell
- Skill kártyáknál tarot kompozíció + stylized background működik legjobban
- `pressed flat against chimney` → megbízható lopakodó póz
- Ékkövek: `smooth curved surface` opálnál, `faceted crystal` a többinél
- Ékkövek alakját mindig explicit add meg: opál=cabochon (smooth curved), zafír=oval faceted cut, rubin=cushion cut (rounded square shape), smaragd=emerald cut (rectangular), gyémánt=round brilliant cut
- `no frame, no border` nem elég → mindig add hozzá: `frameless, borderless, no card border, no decorative edge`
- Pre-renderelt monokróm szín > Godot-tintálás B/W képen: a modulate csak fehér/szürke területeket érint, fekete fekete marad → sötét képeknél (pl. Thief) részletek elvesznek
- The Knight B/W eredeti nem illik a sorozatba: vonalrajz stílus, minimális crosshatching, újragenerálás szükséges
- Skill kártyáknál a Bob Harvey crosshatching + stippling elengedhetetlen a sorozat-konzisztenciához — ha a generált kép "kifestőkönyv" hatású, el kell dobni
- `dense stippling and crosshatching throughout` erősebb hatású mint `detailed stippling and crosshatching`
- Fiatalos/androgün arc elkerüléséhez: `man in his mid-forties, weathered face, strong jaw, grey at temples, battle-scarred` — életkor + fizikai részletek együtt működnek
- Zárt sisak lovag karakternél visszatérő MJ-probléma — többszörös negatív szükséges: `no face covering, no mask, no closed visor, bare face clearly shown`
- Kardmarkolat anatómiája: a keresztvas (quillon) mindig a markolat és a penge találkozásánál van — ha a pengén jelenik meg, az anatómiai hiba, nem díszítés
- Lovas jelenetnél MJ alapból lovon ülő figurát generál — ha a karakter a ló MELLETT áll, explicit add meg és ismételd meg
- Stamina állatoknál természethű ábrázolás kell — nem páncélos, nem felszerelt, nem antropomorf; a természetes állat önmagában elég monumentális
- Farkas monochrome-ban: a sárga/borostyán szempár megjelenik a hideg acélszürke palettán — MJ értelmezési szabadsága, de hatásos; hagyjuk
- Oroszlánnál (és más méltóságteljes állatoknál) nyitott száj = dühös macska benyomása, nem fenség → csukott száj + lefelé néző tekintet + alulnézet adja a valódi méltóságot
- Oroszlán: dombos/sziklás terep érdekesebb a sztereotíp lapos szavannánál — a természetes emelvény vizuálisan is hangsúlyozza a fölényt
- The Rhino → The Gorilla cserére került (22): a rohamozó orrszarú túl hasonló karakterű volt a bikához; a gorilla mellverése teljesen más típusú erőt képvisel
- Potion kártyáknál a három különböző vizuális karakter (gritty/természetes/elegáns) erősíti az egyes statok személyiségét — nem szükséges egységes stílus, elég ha a színek (vörös/zöld/kék) azonosítják a típust
- Redraw kockáknál: kőből izzó/repedező rúnák (lávás technika, dice-2 stílus) nagyon erős alternatíva a sima rúna-körök helyett — érdemes megjegyezni jövőbeli variációhoz