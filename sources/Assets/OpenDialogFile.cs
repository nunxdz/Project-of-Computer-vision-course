using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.IO;
using System.ComponentModel;
using Accord.Statistics.Models.Markov;
using Accord.Statistics.Distributions.Multivariate;
using Accord.Statistics.Models.Markov.Topology;
using System.Linq;
using Accord.Statistics.Models.Markov.Learning;
using Accord.Statistics.Distributions.Fitting;

public class OpenDialogFile : MonoBehaviour
{
    public string filePath = "";
    public GameObject m_goCube;

    private bool isFirst = true;

    // Use this for initialization
    void Start()
    {

        m_goCube = GameObject.Find("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (Main.database.isLoadedFile && isFirst)
        {
            BindingList<Sequence> samples = Main.database.Samples;
            BindingList<string> classes = Main.database.Classes;

            double[][][] inputs = new double[samples.Count][][];
            int[] outputs = new int[samples.Count];

            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = samples[i].Input;
                outputs[i] = samples[i].Output;
            }

            int states = 9;
            int iterations = 0;
            double tolerance = 0.01;
            bool rejection = false;


            Main.hmm = new HiddenMarkovClassifier<MultivariateNormalDistribution, double[]>(classes.Count,
                new Forward(states), new MultivariateNormalDistribution(2), classes.ToArray());


            // Create the learning algorithm for the ensemble classifier
            var teacher = new HiddenMarkovClassifierLearning<MultivariateNormalDistribution, double[]>(Main.hmm)
            {
                // Train each model using the selected convergence criteria
                Learner = i => new BaumWelchLearning<MultivariateNormalDistribution, double[]>(Main.hmm.Models[i])
                {
                    Tolerance = tolerance,
                    MaxIterations = iterations,

                    FittingOptions = new NormalOptions()
                    {
                        Regularization = 1e-5
                    }
                }
            };

            teacher.Empirical = true;
            teacher.Rejection = rejection;


            // Run the learning algorithm
            teacher.Learn(inputs, outputs);


            // Classify all training instances
            foreach (var sample in Main.database.Samples)
            {
                sample.RecognizedAs = Main.hmm.Decide(sample.Input);
            }

            //foreach (DataGridViewRow row in gridSamples.Rows)
            //{
            //    var sample = row.DataBoundItem as Sequence;
            //    row.DefaultCellStyle.BackColor = (sample.RecognizedAs == sample.Output) ?
            //        Color.LightGreen : Color.White;
            //}
            isFirst = false;
            Main.IsLoadDatabase = true;
        }
    }

    void OnGUI()
    {
        GUIContent content = new GUIContent("Select File");
        //Set the GUIStyle style to be label
        GUIStyle style = GUI.skin.GetStyle("button");

        //Set the style font size to increase and decrease over time
        style.fontSize = 30;


        if (GUI.Button(new Rect(110, 950, 180, 70), content, style))
        {

#if UNITY_EDITOR
            filePath = EditorUtility.OpenFilePanel("Load model.."
                                                , Application.streamingAssetsPath
                                                , "xml");
#endif
            if (filePath.Length != 0)
            {
                OpenFilePanel(filePath);

                //texture = new Texture2D(64, 64);
                //www.LoadImageIntoTexture(texture);
                isFirst = true;
            }
        }
    }

    public void OpenFilePanel(string filePath)
    {

        StreamReader stream = new StreamReader(filePath);
        Main.database.Load(stream);
    }
}
