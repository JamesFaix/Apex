export type UserConfig = {
  favoriteWord: string;
};

export type EnvironmentConfig = {
  apiUrl: string;
};

export type Config = {
  environment: EnvironmentConfig;
  user: UserConfig;
};

export const defaultConfig: Config = {
  environment: {
    apiUrl: 'default api url',
  },
  user: {
    favoriteWord: 'default favorite word',
  },
};
