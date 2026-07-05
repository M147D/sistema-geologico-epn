const esc = (s) => String(s ?? '').replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;').replace(/'/g,'&#39;');
const safeColor = (s) => /^#[0-9a-fA-F]{3,8}$/.test(s) ? s : '#999999';

// Genera el HTML del popup geológico como string plano — sin renderToStaticMarkup
// para evitar problemas con react-dom/server en Vite browser builds.
const GeologicalPopup = (properties) => {
  const titulo   = esc(properties.LabelQml  || properties.Leyenda  || 'Formación Geológica');
  const color    = safeColor(properties.ColorRgb);
  const litologia = esc(properties.Litologia || 'No disponible');
  const edad     = esc(properties.Edad      || 'No disponible');

  return `
    <div style="font-family:'Roboto','Helvetica','Arial',sans-serif;min-width:240px;max-width:280px;color:#333;">
      <div style="border-left:5px solid ${color};padding-left:10px;margin-bottom:12px;">
        <h3 style="margin:0;font-size:15px;font-weight:700;color:#2c3e50;line-height:1.2;">${titulo}</h3>
      </div>
      <div style="font-size:12px;color:#444;">
        <div style="margin-bottom:8px;display:flex;align-items:center;gap:8px;">
          <div style="width:20px;height:14px;background-color:${color};border:1px solid rgba(0,0,0,0.2);border-radius:2px;"></div>
          <span style="font-size:11px;color:#777;">Representación</span>
        </div>
        <div style="margin-bottom:8px;padding-left:4px;">
          <div style="color:#555;font-weight:700;margin-bottom:2px;font-size:11px;">LITOLOGÍA:</div>
          <div style="color:#555;line-height:1.4;">${litologia}</div>
        </div>
        <div style="padding-left:4px;">
          <div style="color:#555;font-weight:700;margin-bottom:2px;font-size:11px;">EDAD GEOLÓGICA:</div>
          <div style="color:#555;line-height:1.4;">${edad}</div>
        </div>
      </div>
    </div>`;
};

export default GeologicalPopup;
