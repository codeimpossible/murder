﻿using Bang;
using Bang.Entities;
using Murder.Components;
using Murder.Core;
using Murder.Core.Graphics;
using System.Numerics;

namespace Murder.Services
{
    public static class EffectsServices
    {
        /// <summary>
        /// Add an entity which will apply a "fade-in" effect. Darkening the screen to black.
        /// </summary>
        public static void FadeIn(World world, float time, Color color, float sorting = 0)
        {
            if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
            {
                return;
            }

            var e = world.AddEntity();
            e.SetFadeScreen(new(FadeType.In, Game.NowUnscaled, time, color, sorting: sorting));
        }

        /// <summary>
        /// Add an entity which will apply a "fade-out" effect. Clearing the screen.
        /// </summary>
        public static void FadeOut(World world, float time, Color color, float delay = 0, int bufferDrawFrames = 0)
        {
            if (Game.Instance.IsSkippingDeltaTimeOnUpdate)
            {
                return;
            }

            foreach (var old in world.GetEntitiesWith(typeof(FadeScreenComponent)))
            {
                old.Destroy();
            }

            var e = world.AddEntity();

            if (bufferDrawFrames > 0)
            {
                // With buffer frames we must wait until we get Game.Now otherwise we will get an value
                // specially at lower frame rates
                e.SetFadeScreen(new(FadeType.Out, delay, time, color, string.Empty, 0, bufferDrawFrames));
            }
            else
            {
                e.SetFadeScreen(new(FadeType.Out, Game.NowUnscaled + delay, time, color, string.Empty, 0, bufferDrawFrames));
            }
        }

        public static void ApplyHighlight(World world, Entity e, HighlightSpriteComponent highlight)
        {
            if (e.HasHighlightOnChildren())
            {
                foreach (int childId in e.Children)
                {
                    world.TryGetEntity(childId)?.SetHighlightSprite(highlight);
                }
            }
            else
            {
                e.SetHighlightSprite(highlight);
                e.TryFetchParent()?.SetHighlightSprite(highlight);
            }
        }

        public static void RemoveHighlight(Entity e)
        {
            if (e.HasHighlightOnChildren())
            {
                foreach (int childId in e.Children)
                {
                    e.TryFetchChild(childId)?.RemoveHighlightSprite();
                }
            }
            else
            {
                e.RemoveHighlightSprite();
                e.TryFetchParent()?.RemoveHighlightSprite();
            }
        }

        public static void PlayAnimationAt(World world, Portrait blastAnimation, Vector2 position)
        {
            world.AddEntity(
                new PositionComponent(position),
                new SpriteComponent(blastAnimation),
                new DestroyOnAnimationCompleteComponent()
            );
        }
    }
}