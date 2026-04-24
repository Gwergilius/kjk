# Skill Card Compositing — Technical Specification

## Áttekintés

A kártyák három rétegből állnak össze runtime Godot-ban:

```
[3] Label node-ok     — számok a körökbe + kártyanév a bannerbe (lokalizált)
[2] Keret (frame)     — tintált a kártya színére (modulate)
[1] Figurális kép     — beillesztve a keret Inner Space-ébe
```

---

## Asset-ek

### Skill illusztrációk — `skill/` mappa

Minden kép: **1792×2688px**, PNG, alpha csatorna nélkül (nincs keret).

| Érték | Fájl | Karakter | Keret tint szín | Szám szín |
|------:|------|----------|-----------------|-----------|
| 7 | `07-Fool.png` | The Fool | `#27AE60` smaragdzöld | fekete |
| 8 | `08-Thief.png` | The Thief | `#1F618D` éjkék | fehér |
| 9 | `09-Guard.png` | The Guard | `#CA6F1E` meleg amber | fekete |
| 10 | `10-Champion.png` | The Champion | `#C0392B` krimzon | fehér |
| 11 | `11-Knight.png` | The Knight | `#6C3483` királylila | fehér |
| 12 | `12-Paladin.png` | The Paladin | `#B7950B` meleg arany | fekete |

A szám szín meghatározása: WCAG luminancia-alapú kontraszt (>4.5:1 a megfelelő).

### Keretek — `frames/` mappa

Négy keret variáns, mind **1792×2688px**, PNG **alpha csatornával** (a belső tér átlátszó). A keret fehér ornamentikája a `modulate` tintálással veszi fel a kártya színét; a fekete vonalak feketék maradnak.

| Fájl | Számkörök | Megjegyzés |
|------|-----------|------------|
| `fg-1.png` | 2 (felső-bal, felső-jobb) | Celtic knotwork |
| `fg-2.png` | 4 (mind a 4 sarok) | Art nouveau floral |
| `fg3.png` | 2 (felső-bal, felső-jobb) | Alsó sarkok díszített medalionok maradnak |
| `fg4.png` | 4 (felső-bal, felső-jobb, alsó-bal, alsó-jobb) | Nagy körök |

---

## Keret koordináták (natív 1792×2688px)

### fg-1.png

| Elem | Középpont | Sugár / Méret |
|------|-----------|---------------|
| Left Top kör | (320, 494) | r = 105 px |
| Right Top kör | (1412, 499) | r = 108 px |
| Banner | bal-felső: (566, 2384) | 611 × 126 px |
| Inner Space | bal-felső: (209, 387) | 1275 × 1913 px |

### fg-2.png

| Elem | Középpont | Sugár / Méret |
|------|-----------|---------------|
| Left Top kör | (245, 309) | r = 80 px |
| Right Top kör | (1536, 317) | r = 81 px |
| Left Bottom kör | (247, 2393) | r = 80 px |
| Right Bottom kör | (1534, 2392) | r = 80 px |
| Banner | bal-felső: (640, 2350) | 518 × 96 px |
| Inner Space | bal-felső: (136, 206) | 1491 × 2237 px |

### fg3.png

| Elem | Középpont | Sugár / Méret |
|------|-----------|---------------|
| Left Top kör | (248, 401) | r = 85 px |
| Right Top kör | (1521, 403) | r = 86 px |
| Banner | bal-felső: (598, 2376) | 576 × 84 px |
| Inner Space | bal-felső: (208, 314) | 1403 × 2105 px |

> Megjegyzés: az alsó sarkok szándékosan díszített Celtic medalionok maradtak (nem számkörök).

### fg4.png

| Elem | Középpont | Sugár / Méret |
|------|-----------|---------------|
| Left Top kör | (348, 438) | r = 136 px |
| Right Top kör | (1419, 439) | r = 137 px |
| Left Bottom kör | (215, 2247) | r = 131 px |
| Right Bottom kör | (1295, 2248) | r = 134 px |
| Banner | bal-felső: (541, 2393) | 664 × 102 px |
| Inner Space | bal-felső: (260, 433) | 1270 × 1905 px |

---

## Godot implementáció

### Rétegzés

```
CardRoot (Control vagy Node2D)
├── IllustrationRect   — TextureRect, kép a skill mappából
├── FrameRect          — TextureRect, keret modulate-tal tintálva
├── LabelLeftTop       — Label, szám (ha van)
├── LabelRightTop      — Label, szám (ha van)
├── LabelLeftBottom    — Label, szám (ha van, fg-2 és fg-4)
├── LabelRightBottom   — Label, szám (ha van, fg-2 és fg-4)
└── LabelBanner        — Label, kártyanév (lokalizált)
```

### Illusztráció méretezése és igazítása

Minden Inner Space pontosan **2:3 arányú** (megegyezik az illusztráció 1792×2688 arányával), ezért nincs torzítás — egyszerű arányos skálázás:

```
scale = inner_space_width / 1792          # vagy inner_space_height / 2688
illustration.size = inner_space_size
illustration.position = inner_space_origin
```

### Label pozicionálás

A koordináták a kártya natív felbontásán értendők. Godot-ban a kártya megjelenítési méretéhez lineárisan skálázandók:

```
display_scale = card_display_width / 1792

label.position = circle_center * display_scale
label.pivot_offset = label.size / 2          # középre igazítás
```

### Font és méretek

**Font:** Cinzel Bold (Google Fonts, `.ttf` Godot-ba importálható)  
**Névfelirat:** Cinzel Regular

Ajánlott font méret natív felbontáson (kör sugarából: `font_size ≈ radius * 1.3`):

| Keret | Szám font (natív) | Névfelirat font (natív) |
|-------|-------------------|-------------------------|
| fg-1 | ~136 px | ~69 px |
| fg-2 | ~104 px | ~53 px |
| fg3 | ~111 px | ~46 px |
| fg4 | ~177 px | ~56 px |

Godot-ban: `font_size = native_font_size * display_scale`

A leghosszabb lokalizált szöveg: **"The Champion"** (12 karakter) — ez az irányadó a banner méretezésénél.

### Keret tintálás Godot-ban

```csharp
frameRect.Modulate = skillColor;  // pl. new Color("#27AE60") a Fool-hoz
```

A fehér körök automatikusan a kártya színét veszik fel; a fekete ornamentika fekete marad.
