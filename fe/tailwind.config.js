/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      colors: {
        base: '#080C14',
        surface: '#0F1623',
        elevated: '#1A2235',
        accent: {
          primary: '#6366F1',
          glow: '#818CF8',
          green: '#10B981',
          amber: '#F59E0B',
        },
        primary: '#F1F5F9',
        muted: '#64748B',
        border: '#1E293B',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        display: ['"DM Sans"', 'Inter', 'sans-serif'],
        mono: ['"JetBrains Mono"', 'monospace'],
      },
      boxShadow: {
        glow: '0 0 60px -12px rgba(99, 102, 241, 0.45)',
        'glow-sm': '0 0 30px -8px rgba(99, 102, 241, 0.35)',
        card: '0 4px 24px rgba(0, 0, 0, 0.4)',
      },
      animation: {
        float: 'float 3s ease-in-out infinite',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-8px)' },
        },
      },
      backgroundImage: {
        'hero-gradient': 'linear-gradient(135deg, #6366F1 0%, #A78BFA 100%)',
        'cta-gradient': 'linear-gradient(135deg, #312E81 0%, #1E1B4B 50%, #080C14 100%)',
        'halo': 'radial-gradient(ellipse at center, rgba(99,102,241,0.15) 0%, transparent 70%)',
      },
    },
  },
  plugins: [],
}
