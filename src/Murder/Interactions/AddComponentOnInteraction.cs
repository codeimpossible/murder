﻿using Bang;
using Bang.Components;
using Bang.Entities;
using Bang.Interactions;
using Murder.Attributes;
using Murder.Components;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Interactions
{
    /// <summary>
    /// This will trigger an effect by placing <see cref="Component"/> in the world.
    /// </summary>
    public readonly struct AddComponentOnInteraction : IInteraction
    {
        [NoLabel]
        public readonly IComponent Component;

        [Tooltip("Whether the component will be added on this entity itself.")]
        public readonly TargetEntity Target;

        public void Interact(World world, Entity interactor, Entity? interacted)
        {
            GameLogger.Verify(interacted is not null);

            // We need to guarantee that any modifiable components added here are safe.
            IComponent c = Component is IModifiableComponent ? SerializationHelper.DeepCopy(Component) : Component;

            switch (Target)
            {
                case TargetEntity.Self:
                    interacted.AddOrReplaceComponent(c, c.GetType());
                    break;
                case TargetEntity.Parent:
                    interacted.TryFetchParent()?.AddOrReplaceComponent(c, c.GetType());
                    break;
                case TargetEntity.Interactor:
                    interactor.AddOrReplaceComponent(c, c.GetType());
                    break;
                case TargetEntity.Target:
                    {
                        Entity? target = interacted.TryFindTarget(world, "Target");
                        target?.AddOrReplaceComponent(c, c.GetType());
                        break;
                    }
                case TargetEntity.CreateNewEntity:
                    {
                        Entity e = world.AddEntity(c);

                        // This is created as a child.
                        interacted.AddChild(e.EntityId);

                        // Also propagate the target interaction, if any.
                        if (interacted.TryGetIdTarget() is IdTargetComponent target)
                        {
                            e.SetIdTarget(target);
                        }

                        if (interacted.TryGetIdTargetCollection() is IdTargetCollectionComponent targetCollection)
                        {
                            e.SetIdTargetCollection(targetCollection);
                        }
                        break;
                    }
                default:
                    GameLogger.Warning("Invalid target for Adding a component");
                    break;
            }
            
        }
    }
}