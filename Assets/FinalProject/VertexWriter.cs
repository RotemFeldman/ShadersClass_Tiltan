using UnityEngine;

public class VertexWriter : MonoBehaviour
{
    [Header("References")]
    public Material targetMaterial;
    public Camera mainCamera;
    
    [Header("Texture Settings")]
    public int textureWidth = 512;
    public int textureHeight = 512;
    public Color brushColor = Color.white;
    public float brushSize = 10f;
    public float brushStrength = 1.0f;
    
    private RenderTexture displacementTexture;
    private bool isDrawing = false;
    private Vector2 lastMousePosition;
    
    void Start()
    {
        // Create a new render texture
        displacementTexture = new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
        displacementTexture.enableRandomWrite = true;
        displacementTexture.Create();
        
        // Clear the texture (set to mid-gray for no displacement)
        ClearTexture(Color.gray);
        
        // Assign the render texture to the material
        if (targetMaterial != null)
        {
            targetMaterial.SetTexture("_DisplacementMap", displacementTexture);
        }
    }
    
    void Update()
    {
        if (mainCamera == null)
            return;
            
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lastMousePosition = Input.mousePosition;
            DrawAtPosition(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
        
        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            
            // Draw a line between last position and current position
            DrawLine(lastMousePosition, currentMousePosition);
            
            lastMousePosition = currentMousePosition;
        }
        
        // Clear texture with right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            ClearTexture(Color.gray);
        }
    }
    
    void DrawAtPosition(Vector2 screenPos)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Vector2 texCoord = hit.textureCoord;
                DrawOnTexture(texCoord.x * textureWidth, texCoord.y * textureHeight);
            }
        }
    }
    
    void DrawLine(Vector2 start, Vector2 end)
    {
        // Convert screen positions to rays
        Ray startRay = mainCamera.ScreenPointToRay(start);
        Ray endRay = mainCamera.ScreenPointToRay(end);
        
        RaycastHit startHit, endHit;
        
        if (Physics.Raycast(startRay, out startHit) && Physics.Raycast(endRay, out endHit))
        {
            if (startHit.collider.gameObject == gameObject && endHit.collider.gameObject == gameObject)
            {
                Vector2 startTexCoord = startHit.textureCoord;
                Vector2 endTexCoord = endHit.textureCoord;
                
                // Interpolate between start and end points
                float steps = Vector2.Distance(start, end) * 2; // Double the steps for smooth lines
                steps = Mathf.Max(steps, 10); // At least 10 steps
                
                for (float i = 0; i <= steps; i++)
                {
                    float t = i / steps;
                    Vector2 texCoord = Vector2.Lerp(startTexCoord, endTexCoord, t);
                    DrawOnTexture(texCoord.x * textureWidth, texCoord.y * textureHeight);
                }
            }
        }
    }
    
    void DrawOnTexture(float x, float y)
    {
        // Create a temporary RenderTexture to draw on
        RenderTexture tempRT = RenderTexture.GetTemporary(textureWidth, textureHeight, 0, RenderTextureFormat.ARGB32);
        
        // Copy current displacement texture to temp
        Graphics.Blit(displacementTexture, tempRT);
        
        // Set the temp texture as active render texture
        RenderTexture.active = tempRT;
        
        // Draw on the texture
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, textureWidth, textureHeight, 0);
        
        Texture2D brushTexture = CreateBrushTexture();
        
        // Draw the brush at the specified position
        float halfBrushSize = brushSize * 0.5f;
        Rect brushRect = new Rect(x - halfBrushSize, textureHeight - y - halfBrushSize, brushSize, brushSize);
        Graphics.DrawTexture(brushRect, brushTexture);
        
        GL.PopMatrix();
        
        // Copy the modified texture back to the displacement texture
        Graphics.Blit(tempRT, displacementTexture);
        
        // Clean up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRT);
        DestroyImmediate(brushTexture);
    }
    
    Texture2D CreateBrushTexture()
    {
        int brushTextureSize = Mathf.RoundToInt(brushSize);
        brushTextureSize = Mathf.Max(brushTextureSize, 8); // Minimum size
        
        Texture2D brushTexture = new Texture2D(brushTextureSize, brushTextureSize, TextureFormat.RGBA32, false);
        
        float center = brushTextureSize * 0.5f;
        
        for (int y = 0; y < brushTextureSize; y++)
        {
            for (int x = 0; x < brushTextureSize; x++)
            {
                float distFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float t = Mathf.Clamp01(distFromCenter / center);
                
                // Circular falloff for soft brush
                float alpha = Mathf.Pow(1.0f - t, 2.0f) * brushStrength;
                
                Color pixelColor = brushColor;
                pixelColor.a = alpha;
                brushTexture.SetPixel(x, y, pixelColor);
            }
        }
        
        brushTexture.Apply();
        return brushTexture;
    }
    
    void ClearTexture(Color clearColor)
    {
        // Create a temporary texture with the clear color
        Texture2D clearTexture = new Texture2D(1, 1);
        clearTexture.SetPixel(0, 0, clearColor);
        clearTexture.Apply();
        
        // Set the temp render texture
        RenderTexture tempRT = RenderTexture.GetTemporary(textureWidth, textureHeight);
        
        // Fill the temp texture with the clear color
        Graphics.Blit(clearTexture, tempRT);
        
        // Copy to our displacement texture
        Graphics.Blit(tempRT, displacementTexture);
        
        // Clean up
        RenderTexture.ReleaseTemporary(tempRT);
        DestroyImmediate(clearTexture);
    }
    
    void OnDestroy()
    {
        if (displacementTexture != null)
        {
            displacementTexture.Release();
            DestroyImmediate(displacementTexture);
        }
    }
}