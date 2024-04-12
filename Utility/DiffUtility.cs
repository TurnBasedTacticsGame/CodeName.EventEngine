using System.Text.Json;
using System.Text.Json.Nodes;
using CodeName.Serialization;
using Json.Patch;
using UnityEngine;

namespace CodeName.EventSystem.Utility
{
    public static class DiffUtility
    {
        public static void ValidateGameState<TGameState>(ISerializer serializer, TGameState current, TGameState expected)
        {
            if (HasDifferences(serializer, current, expected, out var currentJson, out var expectedJson))
            {
                Debug.LogWarning("Mismatch between current and expected game state:" +
                    $"\n\nDiff - JSON Patch (RFC 6902): Applying the shown diff on the current state will give the expected state.\n{CreateDiff(currentJson, expectedJson)}\n");
            }
        }

        public static bool ValidateGameState<TGameState>(ISerializer serializer, TGameState current, GameEventNode<TGameState> node)
        {
            var expected = node.ExpectedState;

            if (HasDifferences(serializer, current, expected, out var currentJson, out var expectedJson))
            {
                Debug.LogWarning("Setting current state to expected state. Mismatch between current and expected game state while replaying events:" +
                    $"\n\nDiff - JSON Patch (RFC 6902): Applying the shown diff on the current state will give the expected state.\n{CreateDiff(currentJson, expectedJson)}\n");

                return false;
            }

            return true;
        }

        public static bool HasDifferences<TGameState>(ISerializer serializer, TGameState current, TGameState expected, out string currentJson, out string expectedJson)
        {
            currentJson = serializer.Serialize(current);
            expectedJson = serializer.Serialize(expected);

            return currentJson != expectedJson;
        }

        public static string CreateDiff(string currentJson, string expectedJson)
        {
            var current = JsonNode.Parse(currentJson);
            var expected = JsonNode.Parse(expectedJson);

            var patch = current.CreatePatch(expected);

            return JsonSerializer.Serialize(patch, new JsonSerializerOptions
            {
                WriteIndented = true,
            });
        }
    }
}
