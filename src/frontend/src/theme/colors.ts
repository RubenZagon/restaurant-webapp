// Guachinche Canario Theme Colors
// Inspired by Canarian landscapes: volcanic earth, ocean, and traditional architecture

export const colors = {
  // Primary - Volcanic Earth & Wine
  primary: {
    main: '#8B4513', // Tierra volcánica / Volcanic earth
    light: '#A0522D', // Sienna light
    dark: '#5D2E0F', // Dark earth
    contrast: '#FFFFFF'
  },

  // Secondary - Ocean & Sky
  secondary: {
    main: '#1E88E5', // Atlantic blue
    light: '#42A5F5',
    dark: '#1565C0',
    contrast: '#FFFFFF'
  },

  // Accent - Canarian Wine & Fruits
  accent: {
    wine: '#722F37', // Vino tinto / Red wine
    banana: '#FFD54F', // Plátano canario
    papaya: '#FF6F61', // Papaya/tropical fruit
    mojo: '#C8553D' // Mojo picón red
  },

  // Neutrals - Traditional Architecture
  neutral: {
    white: '#FFFBF0', // Off-white (traditional walls)
    lightGray: '#F5F1E8',
    gray: '#BDB5A5',
    darkGray: '#5D4037',
    black: '#2C1810'
  },

  // Status Colors (maintaining good contrast)
  status: {
    success: '#2E7D32', // Dark green for better contrast
    warning: '#F57C00', // Dark orange
    error: '#C62828', // Dark red
    info: '#1565C0' // Dark blue
  },

  // Background Colors
  background: {
    main: '#FFFBF0', // Warm off-white
    secondary: '#F5F1E8', // Light beige
    paper: '#FFFFFF',
    dark: '#2C1810'
  },

  // Text Colors (WCAG AA compliant)
  text: {
    primary: '#2C1810', // Dark brown (high contrast on light backgrounds)
    secondary: '#5D4037', // Medium brown
    disabled: '#9E9E9E',
    inverse: '#FFFBF0' // For dark backgrounds
  },

  // Border Colors
  border: {
    light: '#E8D5B7',
    main: '#D4A574',
    dark: '#A0784F'
  },

  // Order Status Colors (Kitchen Dashboard)
  orderStatus: {
    pending: {
      bg: '#FFF9E6',
      border: '#FFD54F',
      text: '#5D4037'
    },
    confirmed: {
      bg: '#E3F2FD',
      border: '#42A5F5',
      text: '#1565C0'
    },
    preparing: {
      bg: '#FFF3E0',
      border: '#FF9800',
      text: '#E65100'
    },
    ready: {
      bg: '#E8F5E9',
      border: '#66BB6A',
      text: '#2E7D32'
    },
    delivered: {
      bg: '#F5F1E8',
      border: '#BDB5A5',
      text: '#5D4037'
    }
  }
}

// Helper function to ensure good contrast
export function getTextColor(backgroundColor: string): string {
  // Simple contrast check - in production, use a proper contrast ratio calculation
  const darkBackgrounds = [
    colors.primary.main,
    colors.primary.dark,
    colors.secondary.dark,
    colors.accent.wine,
    colors.background.dark,
    colors.neutral.black,
    colors.neutral.darkGray
  ]

  return darkBackgrounds.includes(backgroundColor)
    ? colors.text.inverse
    : colors.text.primary
}
