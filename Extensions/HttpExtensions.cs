namespace DiplomaWeb.Extensions
{
    public static class HttpExtensions
    {
        /// <summary>
        /// Returns <see cref="HttpContext.User"/> name.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static string GetName(this HttpContext context) {
            try {
                return context.User.Identity!.Name!;
            }
            catch (ArgumentNullException) {
                throw new InvalidOperationException("No user authenticated.");
            }
        }

    }
}
