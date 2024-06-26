//-------------------------------------------------------------------------
// <copyright file="Recipe.cs" company="Universidad Católica del Uruguay">
// Copyright (c) Programación II. Derechos reservados.
// </copyright>
//-------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Full_GRASP_And_SOLID
{
    public class Recipe : IRecipeContent // Modificado por DIP
    {
        // Cambiado por OCP
        private IList<BaseStep> steps = new List<BaseStep>();

        public Product FinalProduct { get; set; }

        // Creo una propiedad Cooked booleana de sólo lectura
        // Ésta propiedad me dirá si la receta ha sido terminada o no
        public bool Cooked { get; set; } = false;

        // Creo el atributo del tipo TimerAdapter
        private TimerAdapter timerClient;

        // Creo el atributo que me va a guardar el timer
        private CountdownTimer timer = new CountdownTimer();

        // Agregado por Creator
        public void AddStep(Product input, double quantity, Equipment equipment, int time)
        {
            Step step = new Step(input, quantity, equipment, time);
            this.steps.Add(step);
        }

        // Agregado por OCP y Creator
        public void AddStep(string description, int time)
        {
            WaitStep step = new WaitStep(description, time);
            this.steps.Add(step);
        }

        public void RemoveStep(BaseStep step)
        {
            this.steps.Remove(step);
        }

        // Agregado por SRP
        public string GetTextToPrint()
        {
            string result = $"Receta de {this.FinalProduct.Description}:\n";
            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetTextToPrint() + "\n";
            }

            // Agregado por Expert
            result = result + $"Costo de producción: {this.GetProductionCost()}";

            return result;
        }

        // Agregado por Expert
        public double GetProductionCost()
        {
            double result = 0;

            foreach (BaseStep step in this.steps)
            {
                result = result + step.GetStepCost();
            }

            return result;
        }

        // Creo un método que me reotrne la suma del tiempo de cada uno de los pasos de una receta
        public int GetCookTime()
        {
            // Genero la variable en donde voy a guardar el tiempo total de la receta
            int tiempoTotal = 0;

            // Itero para cada uno de los pasos en la lista de pasos de la receta (tipo BaseStep)
            foreach (BaseStep step in this.steps)
            {
                // En caso que el paso sea normal,
                // Actualizo la variable tiempoTotal con el tiempo del paso en donde estoy parado
                tiempoTotal += step.Time;
            }

            // Devuelvo el tiempo total de la receta
            return tiempoTotal;
        }

        // Creo el metodo StartCountdown para que comience el conteo
        public void StartCountdown()
        {
            // Creo un nuevo adaptador para el timer
            this.timerClient = new TimerAdapter(this);

            // Registro el timer para que expire cuando la receta termine de cocinarse
            // Ésto es, el tiempo de expiración que le voy a dar al timer va a ser el que se calcule en GetCookTime
            this.timer.Register(this.GetCookTime(), this.timerClient);
        }

        // Creo el metodo void Cook
        // Dentro de éste metodo voy a decirle al timer que empiece el conteo
        public void Cook()
        {
            // Llamo al metodo StartCountdown para que comience la cuenta hacia atrás
            this.StartCountdown();
        }

        // Uso del patrón ADAPTER
        // Creo una clase anidada TimerAdapter la cual implemente la interfaz TimerClient
        private class TimerAdapter : TimerClient
        {
            // Hago que TimeAdapter conozca una instancia Recipe de acceso privado
            private Recipe Recipe;

            // Constructor de la clase anidada TimeAdapter
            public TimerAdapter(Recipe recipe)
            {
                this.Recipe = recipe;
            }

            // Implemento la propiedad de solo lectura TimeOutId
            public object TimeOutId { get; }

            // Implemento el metodo TimeOut definido en la interfaz TimerClient
            // Éste método me va a decir que es lo que ocurre cuando el timer expira
            public void TimeOut()
            {
                // Seteo la propiedad booleana Cooked en true
                this.Recipe.Cooked = true;
            }
        }

    }
}