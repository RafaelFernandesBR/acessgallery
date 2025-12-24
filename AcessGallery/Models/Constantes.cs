using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcessGallery.Models;

public static class Constantes
{
    public const string prompt = @"
Describe the image objectively and in detail, using only the {language} language.  
Follow these rules:
- Do not include introductions, explanations, opinions, or any additional text.
- Provide only the direct and pure description of the visible elements relevants or text in the image.
- Answer **only** in the specified language ({language}).
- If the specified language does not exist, answer in English.

Description examples (follow exactly this format):

1. Empty room with light ceramic flooring and white walls. On the left, there is a peach-colored wall with some exposed wires and a rack with plastic bags hanging from it. In the center, there is a dark wooden door with shutters, which is open, allowing natural light to enter and revealing a balcony with a glass railing. On the right side, there is a pile of transparent plastic heaped on the floor. On the ceiling, there is a lit lamp. In the background, through the balcony, trees and part of a street are visible.

2. A young man is in the kitchen, shirtless, wearing a black cap and colorful shorts. He is cutting vegetables on a cutting board placed on a table covered with a patterned tablecloth. On the board, there are pieces of chopped tomato and cilantro, and he is holding an onion. On the table, there are also some carrots, a black container, and other items wrapped in plastic. In the background, there is a white refrigerator with magnets and papers attached to it. The kitchen has ceramic flooring and a white chair next to the table.

3. Monkey sitting on a rock in a natural environment. The monkey has gray fur with a black face and is looking forward. A striking feature is that its testicles are a vibrant blue color, with a red area in the center. In the background, there is vegetation with bushes and trees.

Now, describe the provided image following exactly the pattern above.
";

    public static readonly Dictionary<string, string> Types = new()
    {
        { "iVBOR", "image/png" },
        { "/9j/", "image/jpeg" },
        { "UklGR", "image/webp" },
        { "AAAB", "image/heic" },
        { "mif1", "image/heif" }
    };
}
