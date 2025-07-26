using System;
using System.Text;

public class PasswordGenerator
{
    public static string GenerateStrongPassword(int length = 12)
    {
        if (length < 8)
            throw new ArgumentException("Password length should be at least 8 characters.");

        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        string allChars = upper + lower + digits + special;
        Random random = new Random();
        StringBuilder password = new StringBuilder();

        // Ensure at least one character from each group
        password.Append(upper[random.Next(upper.Length)]);
        password.Append(lower[random.Next(lower.Length)]);
        password.Append(digits[random.Next(digits.Length)]);
        password.Append(special[random.Next(special.Length)]);

        // Fill the rest with random characters
        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle the result to prevent predictable pattern
        return ShuffleString(password.ToString(), random);
    }

    private static string ShuffleString(string input, Random random)
    {
        char[] array = input.ToCharArray();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array);
    }
}
