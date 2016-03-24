﻿using System.Collections.Generic;

using Microsoft.Bot.Builder.Form.Advanced;

namespace Microsoft.Bot.Builder.Form
{
    public interface IFormModelBuilder<T>
         where T : class, new()
    {
        /// <summary>
        /// Build the form based on the methods called on the builder.
        /// </summary>
        /// <returns>The constructed form.</returns>
        IFormModel<T> Build();

        /// <summary>
        /// The form configuration supplies default templates and settings.
        /// </summary>
        /// <returns>The current form configuration.</returns>
        FormConfiguration Configuration { get; }

        /// <summary>
        /// Show a message that does not require a response.
        /// </summary>
        /// <param name="message">A \ref patterns string to fill in and send.</param>
        /// <param name="condition">Whether or not this step is active.</param>
        /// <returns>This form.</returns>
        IFormModelBuilder<T> Message(string message, ConditionalDelegate<T> condition = null);

        /// <summary>
        /// Show a message with more format control that does not require a response.
        /// </summary>
        /// <param name="prompt">Message to fill in and send.</param>
        /// <param name="condition">Whether or not this step is active.</param>
        /// <returns>This form.</returns>
        IFormModelBuilder<T> Message(Prompt prompt, ConditionalDelegate<T> condition = null);

        /// <summary>
        /// Define a step for filling in a particular value in the form state.
        /// </summary>
        /// <param name="name">Path in the form state to the value being filled in.</param>
        /// <param name="condition">Delegate to test form state to see if step is active.n</param>
        /// <param name="validate">Delegate to validate the field value.</param>
        /// <remarks>
        /// This step will use reflection to construct everything needed for a dialog from a combination
        /// of the <see cref="Describe"/>, <see cref="Terms"/>, <see cref="Prompt"/>, <see cref="Optional"/>
        /// <see cref="Numeric"/> and <see cref="Template"/> annotations that are supplied by default or you
        /// override.
        /// </remarks>
        /// <returns>This form.</returns>
        IFormModelBuilder<T> Field(string name, ConditionalDelegate<T> condition = null, ValidateDelegate<T> validate = null);

        /// <summary>
        /// Define a step for filling in a particular value in the form state.
        /// </summary>
        /// <param name="name">Path in the form state to the value being filled in.</param>
        /// <param name="prompt">Simple \ref patterns to describe prompt for field.</param>
        /// <param name="condition">Delegate to test form state to see if step is active.n</param>
        /// <param name="validate">Delegate to validate the field value.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// This step will use reflection to construct everything needed for a dialog from a combination
        /// of the <see cref="Describe"/>, <see cref="Terms"/>, <see cref="Prompt"/>, <see cref="Optional"/>
        /// <see cref="Numeric"/> and <see cref="Template"/> annotations that are supplied by default or you
        /// override.
        /// </remarks>
        IFormModelBuilder<T> Field(string name, string prompt, ConditionalDelegate<T> condition = null, ValidateDelegate<T> validate = null);

        /// <summary>
        /// Define a step for filling in a particular value in the form state.
        /// </summary>
        /// <param name="name">Path in the form state to the value being filled in.</param>
        /// <param name="prompt">Prompt pattern with more formatting control to describe prompt for field.</param>
        /// <param name="condition">Delegate to test form state to see if step is active.n</param>
        /// <param name="validate">Delegate to validate the field value.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// This step will use reflection to construct everything needed for a dialog from a combination
        /// of the <see cref="Describe"/>, <see cref="Terms"/>, <see cref="Prompt"/>, <see cref="Optional"/>
        /// <see cref="Numeric"/> and <see cref="Template"/> annotations that are supplied by default or you
        /// override.
        /// </remarks>
        IFormModelBuilder<T> Field(string name, Prompt prompt, ConditionalDelegate<T> condition = null, ValidateDelegate<T> validate = null);

        /// <summary>
        /// Derfine a field step by supplying your own field definition.
        /// </summary>
        /// <param name="field">Field definition to use.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// You can provide your own implementation of <see cref="IField<T>"/> or you can 
        /// use the <see cref="Field<T>"/> class to provide fluent values or the <see cref="FieldReflector<T>"/>
        /// to use reflection to provide a base set of values that can be override.  It might 
        /// also make sense to derive from those classes and override the methods you need to 
        /// change.
        /// </remarks>
        IFormModelBuilder<T> Field(IField<T> field);

        /// <summary>
        /// Add all fields not already added to the form.
        /// </summary>
        /// <param name="exclude">Fields not to include.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// This will add all fields defined in your form state that have not already been
        /// added if the fields are supported.
        /// </remarks>
        IFormModelBuilder<T> AddRemainingFields(IEnumerable<string> exclude = null);

        /// <summary>
        /// Add a confirmation step.
        /// </summary>
        /// <param name="prompt">Prompt to use for confirmation.</param>
        /// <param name="condition">Delegate to test if confirmation applies to the current form state.</param>
        /// <param name="dependencies">What fields this confirmation depends on.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// If prompt is not supplied the \ref patterns element {*} will be used to confirm.
        /// Dependencies will by default be all active steps defined before this confirmation.
        /// </remarks>
        IFormModelBuilder<T> Confirm(string prompt = null, ConditionalDelegate<T> condition = null, IEnumerable<string> dependencies = null);

        /// <summary>
        /// Add a confirmation step.
        /// </summary>
        /// <param name="prompt">Prompt to use for confirmation.</param>
        /// <param name="condition">Delegate to test if confirmation applies to the current form state.</param>
        /// <param name="dependencies">What fields this confirmation depends on.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// Dependencies will by default be all active steps defined before this confirmation.
        /// </remarks>
        IFormModelBuilder<T> Confirm(Prompt prompt, ConditionalDelegate<T> condition = null, IEnumerable<string> dependencies = null);

        /// <summary>
        /// Add a confirmation step.
        /// </summary>
        /// <param name="field">Prompt information to use for confirmation.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// This allows you to take full control of the behavior of this confirmation.
        /// </remarks>
        IFormModelBuilder<T> Confirm(IFieldPrompt<T> field);

        /// <summary>
        /// Delegate to call when form is completed.
        /// </summary>
        /// <param name="callback">Delegate to call on completion.</param>
        /// <returns>This form.</returns>
        /// <remarks>
        /// This should only be used for side effects such as calling your service with
        /// the form state results.  In any case the completed form state will be passed
        /// to the parent dialog.
        /// </remarks>
        IFormModelBuilder<T> OnCompletion(CompletionDelegate<T> callback);
    }
}
